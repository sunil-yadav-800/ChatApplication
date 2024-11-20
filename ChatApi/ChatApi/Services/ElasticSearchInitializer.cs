namespace ChatApi.Services
{
    public class ElasticSearchInitializer
    {
        private readonly IElasticSearchService _elasticSearchService;
        public ElasticSearchInitializer(IElasticSearchService elasticSearchService)
        {
            _elasticSearchService = elasticSearchService;
        }
        public async Task InitializeAsync()
        {
            await _elasticSearchService.BulkIndexUsersAsync();
        }
    }
}
