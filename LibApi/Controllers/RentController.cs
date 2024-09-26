using LibApi.DataBaseContext;
using LibApi.Model;
using LibApi.Requests;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public async Task<ActionResult> TakeInRent(RentResigter command)
    {
        var user = _libApi.Users.FirstOrDefault(i => i.Id == command.UserId);
        if (user is null)
            return NotFound($"No user with id {command.UserId}");

        var book = _libApi.BookCopies.FirstOrDefault(i => i.Id == command.BookCopyId);
        if (book is null)
            return NotFound($"No book copy with id {command.UserId}");

        if (book.UserId is not null)
            return BadRequest("That book is already in rent");

        var tariff = _libApi.Tariffs.FirstOrDefault(i => i.IsActive);

        var transaction = new Transaction
        {
            UserId = command.UserId,
            Movement = -(tariff.PaymentPerDay * (decimal)((command.Until - DateTime.Now).TotalHours / 24)),
            TransactionTime = DateTime.Now
        };

        user.Balance += transaction.Movement;

        var currentRents = _libApi.BookRentals.Include(i => i.BookCopy)
            .Where(i => i.IsReturned == false && i.Payment.UserId == user.Id).ToList();

        var totalDeposit = currentRents.Sum(i => i.BookCopy.Cost);

        if (user.Balance - totalDeposit - book.Cost < 0)
            return BadRequest("Need money for deposit and rent");

        await _libApi.Transactions.AddAsync(transaction);
        await _libApi.SaveChangesAsync();

        var rent = new BookRental
        {
            TariffId = tariff.Id,
            BookCopyId = command.BookCopyId,
            RentStart = DateTime.Now,
            RentEnd = command.Until,
            PaymentId = transaction.Id
        };

        book.UserId = command.UserId;
        await _libApi.BookRentals.AddAsync(rent);
        await _libApi.SaveChangesAsync();
        return Ok(new { rentId = rent.Id });
    }

    [HttpPost("{rentId}/return")]
    [Authorize]
    public async Task<ActionResult> ReturnBook(int rentId)
    {
        var rent = _libApi.BookRentals.Include(i => i.BookCopy).FirstOrDefault(i => i.Id == rentId);
        if (rent is null)
            return NotFound($"No rent with id {rentId}");

        rent.IsReturned = true;
        rent.BookCopy.UserId = null;
        rent.BookCopy.User = null;

        await _libApi.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("history/user/{userId}")]
    [Authorize]
    public ActionResult GetHistoryByUser(int userId)
    {
        var user = _libApi.Users.FirstOrDefault(i => i.Id == userId);
        if (user is null)
            return NotFound($"No user with id {userId}");

        return Ok(_libApi.BookRentals.Where(i => i.Payment.UserId == userId).ToList()
            .Select(Utils.TransferData<RentHistoryForUser, BookRental>).ToList());
    }

    [HttpGet("history/book/{copyId}")]
    [Authorize]
    public ActionResult GetHistoryByBook(int copyId)
    {
        var copy = _libApi.BookCopies.FirstOrDefault(i => i.Id == copyId);
        if (copy is null)
            return NotFound($"No book copy with id {copyId}");

        return Ok(_libApi.BookRentals.Where(i => i.BookCopyId == copyId).ToList()
            .Select(Utils.TransferData<RentHistoryForCopy, BookRental>).ToList());
    }

    [HttpGet("current")]
    [Authorize(Roles = "admin")]
    public ActionResult GetCurrentStatuses()
    {
        return Ok(_libApi.BookRentals.Where(i => i.IsReturned == false).Include(i => i.BookCopy)
            .Include(i => i.BookCopy.Book).ToList());
    }

    [HttpPost("{rentId}/loss")]
    [Authorize]
    public async Task<ActionResult> BookCopyLoss(int rentId)
    {
        var rent = _libApi.BookRentals.Include(i => i.BookCopy)
            .Include(bookRental => bookRental.Payment.User).FirstOrDefault(i => i.Id == rentId);
        if (rent is null)
            return NotFound("No rent with that Id");

        if (rent.IsReturned)
            return BadRequest("This book is returned yet");

        rent.BookCopy.IsLost = true;
        rent.Payment.User.Balance -= rent.BookCopy.Cost;
        var transaction = new Transaction
        {
            UserId = rent.Payment.UserId,
            Movement = -rent.BookCopy.Cost,
            TransactionTime = DateTime.Now
        };

        await _libApi.Transactions.AddAsync(transaction);
        await _libApi.SaveChangesAsync();
        return Ok();
    }
}