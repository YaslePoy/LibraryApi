using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LibApi.DataBaseContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibApi.Model;
using LibApi.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;

namespace LibApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        readonly LibApiContext _context;

        public UsersController(LibApiContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(CreateNewUser request)
        {
            var isUserExists = _context.Users.Any(i => i.Login == request.Login);

            if (isUserExists)
                return BadRequest($"There is already user with login {request.Login}");

            var user = Utils.TransferData<User, CreateNewUser>(request);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                userId =
                    user.Id
            });
        }

        [HttpGet("all")]
        public ActionResult GetAllUsers()
        {
            return Ok(_context.Users.ToList().Select(Utils.TransferData<GetUserResponse, User>).ToList());
        }

        [HttpGet("{userId}")]
        public ActionResult GetUserById(int userId)
        {
            var user = _context.Users.FirstOrDefault(i => i.Id == userId);

            if (user is null)
                return NotFound($"No user with id {userId}");

            return Ok(Utils.TransferData<GetUserResponse, User>(user));
        }

        [HttpPatch]
        public async Task<ActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            var user = _context.Users.FirstOrDefault(i => i.Id == request.Id);

            if (user is null)
                return NotFound($"No user with id {request.Id}");

            Utils.TransferData(user, request);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(i => i.Id == userId);

            if (user is null)
                return NotFound($"No user with id {userId}");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{userId}/books")]
        public async Task<ActionResult> GetBooksOfUser(int userId)
        {
            var user = _context.Users.FirstOrDefault(i => i.Id == userId);

            if (user is null)
                return NotFound($"No user with id {userId}");

            var books = _context.BookCopies.Where(i => i.UserId == userId).ToList()
                .Select(Utils.TransferData<BookCopyResponse, BookCopy>);
            return Ok(books);
        }

        [HttpGet("login")]
        public string Authorize(string login, string password)
        {
            var user = _context.Users.FirstOrDefault(i => i.Login == login && i.Password == password);

            if (user is null)
                return "No user with id that login and password";

            var claims = new List<Claim>
            {
                new(ClaimTypes.Authentication, user.Id.ToString()),
                new(ClaimTypes.Role, user.RoleId switch { 0 => "admin", _ => "reader" })
            };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromHours(6)), // время действия 6 часов
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}