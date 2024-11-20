using ChatApi.Models;

namespace ChatApi.Services
{
    public interface IElasticSearchService
    {
        Task IndexUserAsync(UserDto user);
        Task BulkIndexUsersAsync();
        Task<List<UserDto>> SearchUsersAsync(string searchText);
    }
}
