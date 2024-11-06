using ChatApi.Entities;
using ChatApi.Models;
using ChatApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly IMessageRepo _repo;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IMessageRepo repo, ILogger<MessageController> logger)
        {
            _repo = repo;
            _logger = logger;
        }
        [HttpPost("AddMessage")]
        public async Task<string> AddMessage(Message message)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                var res = await _repo.AddMessage(message);
                if (res != null)
                {
                    result.status = true;
                    result.message = "Message Added successfully!";
                    result.data = res;
                }
                else
                {
                    result.status = false;
                    result.message = "Something went wrong while adding messge!";
                }
            }
            catch (Exception ex)
            {
                result.status = false;
                result.message = "Something went wrong while adding message!";
            }
            return JsonConvert.SerializeObject(result);
        }
        [HttpGet("GetMessagesByIds/{loggedInUser}/{otherUser}")]
        public async Task<string> GetMessagesByIds(string loggedInUser, string otherUser)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                var res = await _repo.GetMessagesByIds(new Guid(loggedInUser), new Guid(otherUser));
                result.status = true;
                result.data = res;
            }
            catch (Exception ex)
            {
                result.status = false;
                result.message = "Something went wrong while fetching message!";
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost("MarkMssagesAsSeen")]
        public async Task<string> MarkMssagesAsSeen(List<MessageDto> messages)
        {
            _logger.LogInformation("Start MessageController.MarkMsagesAsSeen method");
            ResponseModel result = new ResponseModel();
            try
            {
                var res = await _repo.MarkMssagesAsSeen(messages);
                result.status = res;
            }
            catch (Exception ex)
            {
                result.status = false;
                result.message = "Something went wrong while marking seen message!";
                _logger.LogError("Exception occured at MessageController.MarkMsagesAsSeen method",ex.ToString());
            }
            _logger.LogInformation("End MessageController.MarkMsagesAsSeen method");
            return JsonConvert.SerializeObject(result);
        }
    }
}
