using LibApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentController : CheckController
{
    [HttpPost("take")]
    [Authorize]
    public async Task<ActionResult> TakeInRent(RentRegister command)
    {
        return null;

        // if (ChechFromJWT(ClaimTypes.Authentication, command.UserId.ToString()) &&
        //     ChechFromJWT(ClaimTypes.Role, "admin"))
        //     return Unauthorized("User can take rent only for his account");
        //
        // if (!_user.IsExists(command.UserId))
        //     return NotFound($"No user with id {command.UserId}");
        //
        // if (!_book.IsCopyExists(command.UserId))
        //     return NotFound($"No book copy with id {command.UserId}");
        //
        // if (_book.GetCopy(command.BookCopyId).UserId is not null)
        //     return BadRequest("That book is already in rent");
        //
        // int rentId = 0;
        // try
        // {
        //     await _rent.TakeInRent(command.UserId, command.BookCopyId, command.Until);
        // }
        // catch (Exception e)
        // {
        //     return BadRequest(e.Message);
        // }
        //
        // return Ok(new { rentId });
    }

    [HttpPost("{rentId}/return")]
    [Authorize]
    public async Task<ActionResult> ReturnBook(int rentId)
    {
        // if (!_rent.IsExists(rentId))
        //     return NotFound($"No rent with id {rentId}");
        // var rent = _rent.Get(rentId);
        // if (ChechFromJWT(ClaimTypes.Authentication, rent.Payment.UserId.ToString()) &&
        //     ChechFromJWT(ClaimTypes.Role, "admin"))
        //     return Unauthorized("User can return copy only for his account");
        //
        //
        // await _rent.Return(rentId);

        return Ok();
    }

    [HttpGet("history/user/{userId}")]
    [Authorize]
    public ActionResult GetHistoryByUser(int userId)
    {
        // if (ChechFromJWT(ClaimTypes.Authentication, userId.ToString()) &&
        //     ChechFromJWT(ClaimTypes.Role, "admin"))
        //     return Unauthorized("User can see history only for his account");
        //
        // if (!_user.IsExists(userId))
        //     return NotFound($"No user with id {userId}");
        //
        // return Ok(_rent.UserHistory(userId).Select(Utils.TransferData<RentHistoryForUser, BookRental>).ToList());
        return null;
    }

    [HttpGet("history/book/{copyId}")]
    [Authorize(Roles = "admin")]
    public ActionResult GetHistoryByBook(int copyId)
    {
        // if (!_book.IsCopyExists(copyId))
        //     return NotFound($"No book copy with id {copyId}");

        return Ok();
    }

    [HttpGet("current")]
    [Authorize(Roles = "admin")]
    public ActionResult GetCurrentStatuses()
    {
        return Ok();
    }

    [HttpPost("{rentId}/loss")]
    [Authorize]
    public async Task<ActionResult> BookCopyLoss(int rentId)
    {
        // var rent = _rent.GetExtended(rentId);
        // if (rent is null)
        //     return NotFound("No rent with that Id");
        //
        // if (rent.IsReturned)
        //     return BadRequest("This book is returned yet");
        //
        // if (ChechFromJWT(ClaimTypes.Authentication, rent.Payment.UserId.ToString()) &&
        //     ChechFromJWT(ClaimTypes.Role, "admin"))
        //     return Unauthorized("User can see history only for his account");
        //
        // await _rent.Loss(rentId);

        return Ok();
    }
}