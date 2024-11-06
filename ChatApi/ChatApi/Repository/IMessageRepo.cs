using ChatApi.Entities;
using ChatApi.Models;

namespace ChatApi.Repository
{
    public interface IMessageRepo
    {
        public Task<Message> AddMessage(Message message);
        public Task<List<MessageDto>> GetMessagesByIds(Guid loggedInUser, Guid otherUser);
        public Task<bool> MarkMssagesAsSeen(List<MessageDto> messages);
    }
}
