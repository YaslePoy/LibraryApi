using LibApi.DataBaseContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibApi.Model;
using LibApi.Requests;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : Controller
    {
        readonly LibApiContext _context;

        public UsersController(LibApiContext context) 
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(CreateNewUser request)
        {
            var isUserExists = _context.Users.Any(i => i.Login == request.Login);

            if (isUserExists)
                return BadRequest($"There is already user with login {request.Login}");

            var user = Utils.TransferData<User, CreateNewUser>(request);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(new {
                userId =
                user.Id
            });
        }
        
        [HttpGet("all")]
        public ActionResult GetAllUsers()
        {
            return Ok(_context.Users.ToList().Select(Utils.TransferData<GetUserResponse, User>).ToList());
        }

        [HttpGet]
        public ActionResult GetUserById(int userId)
        {
            var user = _context.Users.FirstOrDefault(i => i.Id == userId);

            if (user is null)
                return BadRequest($"No user with id {userId}");

            return Ok(Utils.TransferData<GetUserResponse, User>(user));
        }

        [HttpPatch]
        public async Task<ActionResult> UpdateUser([FromQuery] UpdateUserRequest request)
        {
            var user = _context.Users.FirstOrDefault(i => i.Id == request.Id);
            
            if (user is null)
                return BadRequest($"No user with id {request.Id}");

            Utils.TransferData(user, request);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
