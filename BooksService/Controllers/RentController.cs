using System.Security.Claims;
using LibApi.Model;
using LibApi.Requests;
using LibApi.Services.BookService;
using LibApi.Services.RentService;
using LibApi.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentController : CheckController
{
    private readonly IRentService _rent;
    private readonly IUserService _user;
    private readonly IBookService _book;

    public RentController(IRentService rent, IUserService user, IBookService book)
    {
        _rent = rent;
        _user = user;
        _book = book;
    }

    [HttpPost("take")]
    [Authorize]
    public async Task<ActionResult> TakeInRent(RentRegister command)
    {
        if (ChechFromJWT(ClaimTypes.Authentication, command.UserId.ToString()) &&
            ChechFromJWT(ClaimTypes.Role, "admin"))
            return Unauthorized("User can take rent only for his account");

        if (!_user.IsExists(command.UserId))
            return NotFound($"No user with id {command.UserId}");

        if (!_book.IsCopyExists(command.UserId))
            return NotFound($"No book copy with id {command.UserId}");

        if (_book.GetCopy(command.BookCopyId).UserId is not null)
            return BadRequest("That book is already in rent");

        int rentId = 0;
        try
        {
            await _rent.TakeInRent(command.UserId, command.BookCopyId, command.Until);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok(new { rentId });
    }

    [HttpPost("{rentId}/return")]
    [Authorize]
    public async Task<ActionResult> ReturnBook(int rentId)
    {
        if (!_rent.IsExists(rentId))
            return NotFound($"No rent with id {rentId}");
        var rent = _rent.Get(rentId);
        if (ChechFromJWT(ClaimTypes.Authentication, rent.Payment.UserId.ToString()) &&
            ChechFromJWT(ClaimTypes.Role, "admin"))
            return Unauthorized("User can return copy only for his account");


        await _rent.Return(rentId);

        return Ok();
    }

    [HttpGet("history/user/{userId}")]
    [Authorize]
    public ActionResult GetHistoryByUser(int userId)
    {
        if (ChechFromJWT(ClaimTypes.Authentication, userId.ToString()) &&
            ChechFromJWT(ClaimTypes.Role, "admin"))
            return Unauthorized("User can see history only for his account");

        if (!_user.IsExists(userId))
            return NotFound($"No user with id {userId}");

        return Ok(_rent.UserHistory(userId).Select(Utils.TransferData<RentHistoryForUser, BookRental>).ToList());
    }

    [HttpGet("history/book/{copyId}")]
    [Authorize(Roles = "admin")]
    public ActionResult GetHistoryByBook(int copyId)
    {
        if (!_book.IsCopyExists(copyId))
            return NotFound($"No book copy with id {copyId}");

        return Ok(_rent.BookHistory(copyId).Select(Utils.TransferData<RentHistoryForCopy, BookRental>).ToList());
    }
    
    [Authorize(Roles = "admin")]
    [HttpGet("current")]
    public ActionResult GetCurrentStatuses()
    {
        return Ok(_rent.Current().ToList());
    }

    [HttpPost("{rentId}/loss")]
    [Authorize]
    public async Task<ActionResult> BookCopyLoss(int rentId)
    {
        var rent = _rent.GetExtended(rentId);
        if (rent is null)
            return NotFound("No rent with that Id");

        if (rent.IsReturned)
            return BadRequest("This book is returned yet");

        if (ChechFromJWT(ClaimTypes.Authentication, rent.Payment.UserId.ToString()) &&
            ChechFromJWT(ClaimTypes.Role, "admin"))
            return Unauthorized("User can see history only for his account");

        await _rent.Loss(rentId);

        return Ok();
    }
}