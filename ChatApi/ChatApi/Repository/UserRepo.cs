using ChatApi.Entities;
using ChatApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApi.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly ContextDb _db;
        public UserRepo(ContextDb db)
        {
            _db = db;
        }
        public async Task<bool> AddUser(User user)
        {
            try
            {
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                return true;
            }
            catch(Exception ex) 
            {
                return false;
            }
        }
        public async Task<User> AuthenticateAndGetUserInfo(string email, string password)
        {
            try
            {
                var userInfo = await _db.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
                return userInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<UserDto>> GetAllUsers(string userId)
        {
            List<UserDto> users = new List<UserDto>();
            try
            {
                //var result = await _db.Users.ToListAsync();
                var loggedInUserId = new Guid(userId);

                var toUserIds = await (from message in _db.Messages
                                where(message.From == loggedInUserId)
                                select message.To).ToListAsync();

                var fromUserIds = await (from message in _db.Messages
                                where (message.To == loggedInUserId)
                                select message.From).ToListAsync();

                List<Guid> userIds = new List<Guid>();
                foreach(var user  in toUserIds)
                {
                    if(!userIds.Contains(user))
                    { 
                        userIds.Add(user); 
                    }
                }
                foreach(var user in  fromUserIds)
                {
                    if (!userIds.Contains(user))
                    {
                        userIds.Add(user);
                    }
                }

                foreach (var id in userIds)
                {
                    var user = await GetUserById(id);
                    var unreadMessages = 0;
                    if (user.Id != loggedInUserId)
                    {
                        unreadMessages = await _db.Messages.Where(msg => msg.From == user.Id && msg.To == loggedInUserId && msg.IsSeen == false).CountAsync();
                    }
                    UserDto userDto = new UserDto
                    {
                        Id = user.Id.ToString(),
                        Name = user.Name,
                        Email = user.Email,
                        UnreadMessages = unreadMessages
                    };
                    users.Add(userDto);
                }
            }
            catch (Exception ex)
            {
                
            }
            return users;
        }

        public async Task<User> GetUserById(Guid id)
        {
            try
            {
                return await _db.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> RemoveUserById(Guid id)
        {
            try
            {
                User user = await _db.Users.FindAsync(id);
                if(user != null)
                {
                    _db.Remove(user);
                   await _db.SaveChangesAsync();
                }
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> IsEmailAlreadyExist(string email)
        {
            try
            {
                User user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
                return user != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
