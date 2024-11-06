using ChatApi.Entities;
using ChatApi.Models;

namespace ChatApi.Repository
{
    public interface IUserRepo
    {
        public Task<bool> AddUser(User user);
        public Task<User> AuthenticateAndGetUserInfo(string email, string password);
        public Task<bool> RemoveUserById(Guid id);
        public Task<User> GetUserById(Guid id);
        public Task<List<UserDto>> GetAllUsers(string userId);
    }
}
