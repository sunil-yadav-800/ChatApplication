using ChatApi.Entities;
using ChatApi.HelperClass;
using ChatApi.Models;
using ChatApi.Repository;
using ChatApi.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserRepo _userRepo;
        private readonly IConfiguration _configuration;
        private readonly IElasticSearchService _elasticsearchService;
        public AuthController(IUserRepo userRepo, IConfiguration configuration, IElasticSearchService elasticsearchService)
        {
            _userRepo = userRepo;
            _configuration = configuration;
            _elasticsearchService = elasticsearchService;
        }

        [HttpPost("Register")]
        public async Task<string> Register(Registeration registeration)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                var newUser = new User()
                {
                    Name = registeration.Name,
                    Email = registeration.Email,
                    Password = registeration.Password,
                };
                bool isUserExist = await _userRepo.IsEmailAlreadyExist(newUser.Email);
                if (!isUserExist)
                {
                    var res = await _userRepo.AddUser(newUser);
                    if (res)
                    {
                        User registeredUser = await _userRepo.AuthenticateAndGetUserInfo(newUser.Email, newUser.Password);
                        if (registeredUser != null)
                        {
                            UserDto user = new UserDto
                            {
                                Id = registeredUser.Id.ToString(),
                                Name = registeredUser.Name,
                                Email = registeredUser.Email,
                            };
                            await _elasticsearchService.IndexUserAsync(user);
                        }
                        result.status = true;
                        result.message = "User registration successful";
                    }
                    else
                    {

                        result.status = false;
                        result.message = "User registration Unsuccessful";

                    }
                }
                else
                {
                    result.status = false;
                    result.message = "Email is already taken. Please try with another emil";
                }
            }
            catch(Exception ex)
            {

                result.status = false;
                result.message = "Error while Registration";
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost("Login")]
        public async Task<string> Login(Login login)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                var userInfo =  await _userRepo.AuthenticateAndGetUserInfo(login.Email, login.Password);
                if(userInfo != null)
                {
                    var token = new Helper(_configuration).GenerateToken(userInfo.Id.ToString(),userInfo.Name,userInfo.Email);
                    var user = new
                    {
                        userId = userInfo.Id,
                        email = userInfo.Email,
                        name = userInfo.Name
                    };
                    var userData = new
                    {
                        user = user,
                        token = token
                    };
                    result.status = true;
                    result.data = userData;
                }
                else
                {
                    result.status = false;
                    result.message = "Invalid Credentials";
                }
            }
            catch(Exception ex)
            {
                result.status = false;
                result.message = "Error while login";
                
            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpGet("GetAllUsers/{userId}")]
        public async Task<string> GetAllUsers(string userId)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                var users = await _userRepo.GetAllUsers(userId);
                if (users != null)
                {
                    result.status = true;
                    result.data = users;
                }

            }
            catch (Exception ex)
            {
                result.status = false;
                result.message = "Error while fetching all users";

            }
            return JsonConvert.SerializeObject(result);
        }

        [HttpGet("SearchUsers/{searchTerm}")]
        public async Task<string> SearchUsers(string searchTerm)
        {
            ResponseModel result = new ResponseModel();
            try
            {
                var users = await _elasticsearchService.SearchUsersAsync(searchTerm);
                result.status = true;
                result.data = users;
            }
            catch (Exception ex)
            {
                result.status = false;
                result.message = "Error while searching users";

            }
            return JsonConvert.SerializeObject(result);
        }
    }
}
