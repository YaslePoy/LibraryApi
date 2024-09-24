﻿using LibApi.DataBaseContext;
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

        // [HttpGet]
        // [Route("getAllUsers")]
        // public async Task<IActionResult> GetAllUsers()
        // {
        //     var users = await _context.Logins.Where(a => a.id_Login == 1).Include(a => a.Users).ToListAsync();
        //      
        //     return new OkObjectResult(new
        //     {
        //         users = users,
        //         status = true
        //     });
        // }
        //
        // [HttpPost]
        // [Route("createNewUserAndLogin")]
        // public async Task<IActionResult> CreateNewUserAndLogin(CreateNewUserAndLogin newUser)
        // {
        //     var user = new User()
        //     {
        //         Name = newUser.Name,
        //         Description = newUser.Description,
        //     };
        //
        //     await _context.Users.AddAsync(user);
        //     await _context.SaveChangesAsync();
        //
        //     var login = new Logins()
        //     {
        //         User_id = user.id_User,
        //         Login = newUser.Login,
        //         Password = newUser.Password,
        //     };
        //
        //     await _context.Logins.AddAsync(login);
        //     await _context.SaveChangesAsync();
        //
        //     return Ok();
        // }
    }
}