using ChatApi.Models;
using System.Collections.Concurrent;

namespace ChatApi.HelperClass
{
    public class UserConnectionManager
    {
        private ConcurrentDictionary<string, string> connections = new ConcurrentDictionary<string, string>();

        private ConcurrentDictionary<string, UserDto> onlineUsers = new ConcurrentDictionary<string, UserDto>();

        public void AddUserConnection(string connectionId, string userId, UserDto user)
        {
            try
            {
                connections.TryAdd(connectionId, userId);
                onlineUsers.TryAdd(connectionId, user);
            }
            catch(Exception ex)
            {

            }
        }
        public void RemoveUserConnection(string connectionId)
        {
            try
            {
                connections.TryRemove(connectionId, out var connValue);
                onlineUsers.TryRemove(connectionId, out var userValue);
            }
            catch (Exception ex)
            {

            }
        }
        public List<UserDto> GetAllConnectedUsers()
        {
            List<UserDto> connectedUsers = new List<UserDto>();
            try
            {
                connectedUsers = onlineUsers.Values.ToList();
            }
            catch (Exception ex)
            {

            }
            return connectedUsers;
        }
        public string GetConnectionIdFromUserId(string userId)
        {
            string connectionId = string.Empty;
            try
            {
                connectionId = connections.FirstOrDefault(u => u.Value.Equals(userId)).Key;
            }
            catch(Exception ex)
            {

            }
            return connectionId;
        }
    }
}
