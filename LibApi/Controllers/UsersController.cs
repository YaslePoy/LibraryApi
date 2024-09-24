using LibApi.DataBaseContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibApi.Model;
using LibApi.Requests;

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
        
        
    }
}
