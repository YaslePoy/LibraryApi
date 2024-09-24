using LibApi.DataBaseContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibApi.Model;
using LibApi.Requests;

namespace LibApi.Controllers
{
    [ApiController]
    [Route("api/user")]
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

            var user = Utils.CreateDBEntity<User, CreateNewUser>(request);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(new {
                userId =
                user.Id
            });
        }
    }
}
