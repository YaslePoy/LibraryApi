using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestApi2.DataBaseContext;
using TestApi2.Model;
using TestApi2.Requests;

namespace TestApi2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        readonly TestApiDB _context;

        public UsersController(TestApiDB context) 
        {
            _context = context;
        }

        [HttpGet]
        [Route("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Logins.Where(a => a.id_Login == 1).Include(a => a.Users).ToListAsync();
             
            return new OkObjectResult(new
            {
                users = users,
                status = true
            });
        }

        [HttpPost]
        [Route("createNewUserAndLogin")]
        public async Task<IActionResult> CreateNewUserAndLogin(CreateNewUserAndLogin newUser)
        {
            var user = new Users()
            {
                Name = newUser.Name,
                Description = newUser.Description,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var login = new Logins()
            {
                User_id = user.id_User,
                Login = newUser.Login,
                Password = newUser.Password,
            };

            await _context.Logins.AddAsync(login);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
