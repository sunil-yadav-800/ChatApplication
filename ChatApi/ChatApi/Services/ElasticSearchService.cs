using ChatApi.Entities;
using ChatApi.Models;
using Nest;

namespace ChatApi.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly IServiceProvider _serviceProvider;
        public ElasticSearchService(IElasticClient elasticClient, IServiceProvider serviceProvider)
        {
            _elasticClient = elasticClient;
            _serviceProvider = serviceProvider;
        }
        public async Task BulkIndexUsersAsync()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ContextDb>();
                    var existingUsers = dbContext.Users.Select(u => new UserDto
                    {
                        Id = u.Id.ToString(),
                        Name = u.Name,
                        Email = u.Email,
                    }).ToList();

                    var bulkRequest = new BulkDescriptor();
                    foreach (var user in existingUsers)
                    {
                        bulkRequest.Index<UserDto>(x => x.Document(user));
                    }
                    await _elasticClient.BulkAsync(bulkRequest);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save message: " + ex.Message);
            }
        }
        public async Task IndexUserAsync(UserDto user)
        {
           await _elasticClient.IndexDocumentAsync(user);
        }

        public async Task<List<UserDto>> SearchUsersAsync(string searchText)
        {

            //prefix search
            var searchResponse = await _elasticClient.SearchAsync<UserDto>(s => s
                       .Query(q => q
                           .Bool(b => b
                               .Should(
                                   m => m.Prefix(p => p
                                       .Field(f => f.Name)
                                       .Value(searchText)
                                   ),
                                   m => m.Prefix(p => p
                                       .Field(f => f.Email)
                                       .Value(searchText)
                                   )
                               )
                           )
                       )
                   );

            //for full text search
            if (searchResponse == null || searchResponse.Documents.Count == 0)
            {
                searchResponse = await _elasticClient.SearchAsync<UserDto>(s => s
                    .Query(q => q
                        .MultiMatch(m => m
                            .Fields(f => f
                                .Field(p => p.Name)
                                .Field(p => p.Email)
                            )
                            .Query(searchText)
                            .Fuzziness(Fuzziness.Auto)
                        )
                    )
                );
            }

            return searchResponse.Documents.ToList();
        }
    }
}
