using ChatApi.Controllers;
using ChatApi.Entities;
using ChatApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ChatApi.Repository
{
    public class MessageRepo : IMessageRepo
    {
        private readonly ContextDb _db;
        private readonly ILogger<MessageController> _logger;
        public MessageRepo(ContextDb db, ILogger<MessageController> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<Message> AddMessage(Message message)
        {
            try
            {
                _db.Messages.Add(message);
                await _db.SaveChangesAsync();
                return message;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<MessageDto>> GetMessagesByIds(Guid loggedInUser, Guid otherUser)
        {
            List<MessageDto> msgs = new List<MessageDto>();
            try
            {
                var messages = from message in _db.Messages
                               where (message.From == loggedInUser &&  message.To == otherUser)
                                    ||(message.From == otherUser && message.To == loggedInUser)
                               orderby message.CreatedAt 
                               select message;

                //var allMessages = await messages.ToListAsync();
                foreach(var msg in messages)
                {
                    MessageDto msgDto = new MessageDto
                    {
                        Id = msg.Id.ToString(),
                        Content = msg.Content,
                        From = msg.From.ToString(),
                        To = msg.To.ToString(),
                        IsSeen = msg.IsSeen,
                        CreatedAt = msg.CreatedAt,
                    };
                    msgs.Add(msgDto);
                }       
            }
            catch (Exception ex)
            {
                return null;
            }
            return msgs;
        }
        public async Task<bool> MarkMssagesAsSeen(List<MessageDto> messages)
        {
            _logger.LogInformation("Start MessageRepo.MarkMsagesAsSeen method");
            try
            {
                var userMessages = new List<Message>();
                foreach (var message in messages)
                {
                    var msg = await _db.Messages.FirstOrDefaultAsync(msg => msg.Id == new Guid(message.Id));

                    if (msg != null)
                    {
                        userMessages.Add(msg);
                    }
                }

                foreach (var message in userMessages)
                {
                    message.IsSeen = true;
                }

                await _db.SaveChangesAsync();
                _logger.LogInformation("End MessageRepo.MarkMsagesAsSeen method");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured at MessageController.MarkMsagesAsSeen method", ex.ToString());
                return false;
            }
        }
    }
}
