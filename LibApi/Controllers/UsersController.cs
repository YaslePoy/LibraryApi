﻿using Microsoft.AspNetCore.Mvc;
using LibApi.Requests;
using Microsoft.AspNetCore.Authorization;

namespace LibApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : CheckController
    {
        // private readonly IUserService _user;
        //
        // public UsersController(IUserService user)
        // {
        //     _user = user;
        // }

        [HttpGet("login")]
        public string Authorize(string login, string password)
        {

            // if(!_user.IsLoginExists(login))
            //     return "No user with id that login and password";
            //
            // return _user.Authorize(login, password);
            return null;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(CreateNewUser request)
        {
            return null;
            // if (string.IsNullOrWhiteSpace(request.Name))
            //     return BadRequest("Unexpected name");
            //
            // if (string.IsNullOrWhiteSpace(request.Login))
            //     return BadRequest("Unexpected loign");
            //
            // if (string.IsNullOrWhiteSpace(request.Password))
            //     return BadRequest("Unexpected password");
            //
            // if (string.IsNullOrWhiteSpace(request.Surname))
            //     return BadRequest("Unexpected surname");
            //
            // if (string.IsNullOrWhiteSpace(request.Phone))
            //     return BadRequest("Unexpected phone");
            //
            //
            // if (_user.IsLoginExists(request.Login))
            //     return BadRequest($"There is already user with login {request.Login}");
            //
            // return Ok(new
            // {
            //     userId =
            //         _user.Create(request)
            // });
        }

        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public ActionResult GetAllUsers()
        {
            return null;
            // return Ok(_user.GetAll().Select(Utils.TransferData<GetUserResponse, User>).ToList());
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "admin")]
        public ActionResult GetUserById(int userId)
        {
            // var user = _user.Get(userId);
            //
            // if (user is null)
            //     return NotFound($"No user with id {userId}");
            //
            // return Ok(Utils.TransferData<GetUserResponse, User>(user));
            return null;
        }

        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            // if (string.IsNullOrWhiteSpace(request.Name))
            //     return BadRequest("Unexpected name");
            //
            // if (string.IsNullOrWhiteSpace(request.Surname))
            //     return BadRequest("Unexpected surname");
            //
            // if (string.IsNullOrWhiteSpace(request.Phone))
            //     return BadRequest("Unexpected phone");
            //
            // if (ChechFromJWT(ClaimTypes.Authentication, request.Id.ToString()) &&
            //     ChechFromJWT(ClaimTypes.Role, "admin"))
            //     return Unauthorized("User can update profile only for his account");
            //
            // if (!_user.IsExists(request.Id))
            //     return NotFound($"No user with id {request.Id}");
            //
            // await _user.Update(request.Id, request);

            return Ok();
        }

        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<ActionResult> DeleteUser(int userId)
        {
            // if (ChechFromJWT(ClaimTypes.Authentication, userId.ToString()) &&
            //     ChechFromJWT(ClaimTypes.Role, "admin"))
            //     return Unauthorized("User can delete profile only for his account");
            //
            //
            // if (!_user.IsExists(userId))
            //     return NotFound($"No user with id {userId}");
            //
            // await _user.Delete(userId);
            
            return Ok();
        }

        [HttpGet("{userId}/books")]
        [Authorize]
        public async Task<ActionResult> GetBooksOfUser(int userId)
        {

            return Ok();
        }
    }
}