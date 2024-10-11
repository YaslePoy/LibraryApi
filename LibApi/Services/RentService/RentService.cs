using LibApi.DataBaseContext;
using LibApi.Model;
using LibApi.Services.BookService;
using LibApi.Services.UserService;
using Microsoft.EntityFrameworkCore;

namespace LibApi.Services.RentService;

public class RentService : IRentService
{
    private readonly LibApiContext _libApi;
    private readonly IUserService _user;
    private readonly IBookService _book;

    public RentService(LibApiContext libApi, IUserService user, IBookService book)
    {
        _libApi = libApi;
        _user = user;
        _book = book;
    }

    public async Task<int> TakeInRent(int userId, int bookCopyId, DateTime until)
    {
        var tariff = _libApi.Tariffs.FirstOrDefault(i => i.IsActive);

        var transaction = new Transaction
        {
            UserId = userId,
            Movement = -(tariff.PaymentPerDay * (decimal)((until - DateTime.Now).TotalHours / 24)),
            TransactionTime = DateTime.Now
        };
        var user = _user.Get(userId);
        user.Balance += transaction.Movement;

        var currentRents = _libApi.BookRentals.Include(i => i.BookCopy)
            .Where(i => i.IsReturned == false && i.Payment.UserId == user.Id).ToList();

        var totalDeposit = currentRents.Sum(i => i.BookCopy.Cost);
        var book = _book.GetCopy(bookCopyId);
        if (user.Balance - totalDeposit - book.Cost < 0)
            throw new Exception("Need money for deposit and rent");

        await _libApi.Transactions.AddAsync(transaction);
        await _libApi.SaveChangesAsync();

        var rent = new BookRental
        {
            TariffId = tariff.Id,
            BookCopyId = bookCopyId,
            RentStart = DateTime.Now,
            RentEnd = until,
            PaymentId = transaction.Id
        };

        book.UserId = userId;
        await _libApi.BookRentals.AddAsync(rent);
        await _libApi.SaveChangesAsync();
        return rent.Id;
    }

    public async Task Return(int rentId)
    {
        var rent = _libApi.BookRentals.Include(i => i.BookCopy).FirstOrDefault(i => i.Id == rentId);
        rent.IsReturned = true;
        rent.BookCopy.UserId = null;
        rent.BookCopy.User = null;

        await _libApi.SaveChangesAsync();
    }

    public IReadOnlyList<BookRental> UserHistory(int userId)
    {
        return _libApi.BookRentals.Where(i => i.Payment.UserId == userId).ToList();
    }

    public IReadOnlyList<BookRental> BookHistory(int bookId)
    {
        return _libApi.BookRentals.Where(i => i.BookCopyId == bookId).ToList();
    }

    public IReadOnlyList<BookRental> Current()
    {
        return _libApi.BookRentals.Where(i => i.IsReturned == false).Include(i => i.BookCopy)
            .Include(i => i.BookCopy.Book).ToList();
    }

    public async Task Loss(int rentId)
    {
        var rent = GetExtended(rentId);
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
    }

    public BookRental Get(int id)
    {
        return _libApi.BookRentals.FirstOrDefault(i => i.Id == id);
    }

    public BookRental? GetExtended(int id)
    {
        return _libApi.BookRentals.Include(i => i.BookCopy)
            .Include(bookRental => bookRental.Payment.User).FirstOrDefault(i => i.Id == id);
        
    }

    public bool IsExists(int id)
    {
        return _libApi.BookRentals.Any(i => i.Id == id);
    }
}