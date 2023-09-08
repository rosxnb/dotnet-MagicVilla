using Microsoft.AspNetCore.Mvc;
using API.Repository.IRepository;
using API.DTOs;
using API.Models;
using System.Net;

namespace API.Controllers
{
    [ApiController]
    [Route("api/UsersAuth")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRespository _userRepository;
        private readonly APIResponse _response;

        public UsersController(IUserRespository userRespository)
        {
            _userRepository = userRespository;
            _response = new APIResponse
            {
                ErrorMessages = new List<string>()
            };
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            LoginResponseDTO responseDTO = await _userRepository.Login(model);
            if (string.IsNullOrEmpty(responseDTO.Token) || responseDTO.User is null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSucess = false;
                _response.ErrorMessages!.Add("Username or password is incorrect.");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSucess = true;
            _response.Result = responseDTO;
            return Ok(_response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            bool isUnique = _userRepository.IsUniqueUser(model.UserName);
            if (!isUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSucess = false;
                _response.ErrorMessages!.Add("Username alredy exists.");
                return BadRequest(_response);
            }

            LocalUser user = await _userRepository.Register(model);
            if (user is null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSucess = false;
                _response.ErrorMessages!.Add("Registration error.");
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSucess = false;
            return Ok(_response);
        }
    }
}
