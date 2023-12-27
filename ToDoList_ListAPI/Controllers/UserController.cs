using Microsoft.AspNetCore.Mvc;
using System.Net;
using ToDoList_ListAPI.Models.DTO;
using ToDoList_ListAPI.Models;
using ToDoList_ListAPI.Repository.IRepository;
using Azure;
using Microsoft.AspNetCore.Authorization;

namespace ToDoList_ListAPI.Controllers
{
    public class UserController : Controller
    {

        private readonly IUserRepository _userRepo;
        protected APIResponse _response;
        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
            _response = new();
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)] 
        [ProducesResponseType(typeof(LoginResponseDTO), (int)HttpStatusCode.OK)] 
        [AllowAnonymous]

        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            if (model.UserName == null || model.Password == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Please fill in username and password");
                return BadRequest(_response);
            }
            var loginResponse = await _userRepo.Login(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username or password is incorrect");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)] 
        [ProducesResponseType(typeof(LocalUser), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            if (model.UserName == null || model.Password == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Please fill in username and password");
                return BadRequest(_response);
            }
            bool ifUserNameUnique = _userRepo.IsUniqueUser(model.UserName);
            if (!ifUserNameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exists");
                return BadRequest(_response);
            }

            var user = await _userRepo.Register(model);
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while registering");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }

        [HttpPost("forgotPassword")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)] 
        [ProducesResponseType(typeof(LocalUser), (int)HttpStatusCode.OK)]

        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDTO model)
        {
            if (model.Email == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Please fill in email");
                return BadRequest(_response);
            }
            var forgetPasswordResponse = await _userRepo.ForgotPassword(model);
            if (forgetPasswordResponse.User == null || forgetPasswordResponse.Email == "")
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Email does not exist");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }

    }
}
