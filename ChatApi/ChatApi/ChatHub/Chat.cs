using ChatApi.Entities;
using ChatApi.HelperClass;
using ChatApi.Models;
using ChatApi.Repository;
using ChatApi.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChatApi.ChatHub
{
    public class Chat : Hub
    {
        private UserConnectionManager _userConnectionManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ProducerService _producerService;
        public Chat(UserConnectionManager userConnectionManager, IServiceProvider serviceProvider, ProducerService producerService)
        {
            _userConnectionManager = userConnectionManager;
            _serviceProvider = serviceProvider;
            _producerService = producerService;
        }
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            //var userName = Context.User?.Identity?.Name ?? "Anonymous";
            var httpContext = Context.GetHttpContext();
            var token = httpContext?.Request?.Query["access_token"].ToString();
            if(!string.IsNullOrEmpty(token) )
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var userId = jsonToken?.Claims?.FirstOrDefault(claim=>claim.Type == ClaimTypes.NameIdentifier)?.Value;
                var userEmail = jsonToken?.Claims?.FirstOrDefault(claim => claim.Type == "email")?.Value;
                var userName = jsonToken?.Claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
                UserDto user = new UserDto { Id = userId, Email = userEmail, Name = userName };

                _userConnectionManager.AddUserConnection(connectionId, userId, user);
            }

            await Clients.All.SendAsync("OnlineUsers",_userConnectionManager.GetAllConnectedUsers());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;

            _userConnectionManager.RemoveUserConnection(connectionId);

            await Clients.All.SendAsync("OnlineUsers", _userConnectionManager.GetAllConnectedUsers());
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(MessageDto messageDto)
        {
            Message newMessage = new Message
            {
                Id = Guid.NewGuid(),
                Content = messageDto.Content,
                From = new Guid(messageDto.From),
                To = new Guid(messageDto.To),
                CreatedAt = DateTime.Now,
                IsSeen = false
            };

            messageDto.Id = newMessage.Id.ToString();
            messageDto.CreatedAt = newMessage.CreatedAt;
            messageDto.IsSeen = false;

            string toUserConnectnId = _userConnectionManager.GetConnectionIdFromUserId(messageDto.To);

            await Clients.Caller.SendAsync("ReceiveMessage", messageDto);
            if (!string.IsNullOrEmpty(toUserConnectnId))
            {
                await Clients.User(messageDto.To).SendAsync("ReceiveMessage", messageDto);
            }

            //add message to the db async
            Task saveMsgToDB = SaveMessageToDb(newMessage);
        }

        public async Task NotifySenderAboutMessageSeen(string senderUserId, string recieverUserId)
        {
            string senderConnectionId = _userConnectionManager.GetConnectionIdFromUserId(senderUserId);
            if (!string.IsNullOrEmpty(senderConnectionId))
            {
                var res = new
                {
                    senderUserId,
                    recieverUserId
                };
                await Clients.User(senderUserId).SendAsync("MessageSeen", res);
            }
        }
        private async Task SaveMessageToDb(Message message)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ContextDb>();
                    dbContext.Messages.Add(message);
                    await dbContext.SaveChangesAsync();
                }

                //await _producerService.ProduceAsync("message", message);
            }
            catch (Exception ex)
            {
                // Handle exception (log, etc.)
                Console.WriteLine("Failed to save message: " + ex.Message);
            }
        }
    }
}
