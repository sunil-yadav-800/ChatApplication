using ChatApi.ChatHub;
using ChatApi.Entities;
using ChatApi.HelperClass;
using ChatApi.Models;
using ChatApi.Repository;
using ChatApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nest;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", policy =>
    {
        policy.WithOrigins("http://localhost:4300")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});
builder.Services.AddDbContext<ContextDb>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddSignalR();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepo,UserRepo>();
builder.Services.AddScoped<IMessageRepo, MessageRepo>();
builder.Services.AddSingleton<UserConnectionManager>();
builder.Services.AddSingleton<ProducerService>();
builder.Services.AddHostedService<ConsumerService>();

//add elastic search
builder.Services.AddSingleton<IElasticClient>(sp =>
{
    var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
        .DefaultIndex("users") // Default index name
        .DefaultMappingFor<UserDto>(m => m
            .PropertyName(p => p.Id, "id")
            .PropertyName(p => p.Name, "name")
            .PropertyName(p => p.Email, "email")
        );

    return new ElasticClient(settings);
});

builder.Services.AddScoped<IElasticSearchService, ElasticSearchService>();
builder.Services.AddScoped<ElasticSearchInitializer>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOption =>
    {
        var key = builder.Configuration.GetValue<string>("Jwt:Key");
        var keyBytes = Encoding.ASCII.GetBytes(key);
        jwtOption.SaveToken = true;
        jwtOption.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false
        };
        jwtOption.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our SignalR hub
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/Chat")))  // Adjust the path if your hub endpoint is different
                {
                    // Read the token from the query string
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
});
builder.Services.AddSignalR();

//addlogging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var elasticSearchInitializer = scope.ServiceProvider.GetRequiredService<ElasticSearchInitializer>();
    await elasticSearchInitializer.InitializeAsync();
}

app.UseCors("cors");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<Chat>("/Chat");

app.Run();
