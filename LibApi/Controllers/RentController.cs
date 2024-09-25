using LibApi.DataBaseContext;
using LibApi.Model;
using LibApi.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentController : Controller
{
    private readonly LibApiContext _libApi;

    public RentController(LibApiContext libApi)
    {
        _libApi = libApi;
    }

    [HttpPost("take")]
    public async Task<ActionResult> TakeInRent(RentResigter command)
    {
        var user = _libApi.Users.FirstOrDefault(i => i.Id == command.UserId);
        if (user is null)
            return BadRequest($"No user with id {command.UserId}");

        var book = _libApi.BookCopies.FirstOrDefault(i => i.Id == command.BookCopyId);
        if (book is null)
            return BadRequest($"No book copy with id {command.UserId}");

        var rent = new BookRental
        {
            UserId = command.UserId,
            BookCopyId = command.BookCopyId,
            RentStart = DateTime.Now,
            RentEnd = command.Until
        };

        await _libApi.BookRentals.AddAsync(rent);
        await _libApi.SaveChangesAsync();
        return Ok(new { rentId = rent.Id });
    }

    [HttpPost("{rentId}/return")]
    public async Task<ActionResult> ReturnBook(int rentId)
    {
        var rent = _libApi.BookRentals.Include(i => i.BookCopy).FirstOrDefault(i => i.Id == rentId);
        if (rent is null)
            return BadRequest($"No rent with id {rentId}");

        rent.IsReturned = true;
        rent.BookCopy.UserId = null;
        rent.BookCopy.User = null;

        await _libApi.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("history/user/{userId}")]
    public ActionResult GetHistoryByUser(int userId)
    {
        var user = _libApi.Users.FirstOrDefault(i => i.Id == userId);
        if (user is null)
            return BadRequest($"No user with id {userId}");

        return Ok(_libApi.BookRentals.Where(i => i.UserId == userId).Include(i => i.BookCopy)
            .Include(i => i.BookCopy.Book).ToList());
    }

    [HttpGet("history/book/{copyId}")]
    public ActionResult GetHistoryByBook(int copyId)
    {
        var copy = _libApi.BookCopies.FirstOrDefault(i => i.Id == copyId);
        if (copy is null)
            return BadRequest($"No book copy with id {copyId}");

        return Ok(_libApi.BookRentals.Where(i => i.BookCopyId == copyId).Include(i => i.User).ToList());
    }

    [HttpGet("current")]
    public ActionResult GetCurrentStatuses()
    {
        return Ok(_libApi.BookRentals.Where(i => i.IsReturned == false).Include(i => i.User).Include(i => i.BookCopy)
            .Include(i => i.BookCopy.Book).ToList());
    }
}