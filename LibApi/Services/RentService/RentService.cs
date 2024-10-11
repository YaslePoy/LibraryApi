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

    public Task Return(int rentId)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<BookRental> UserHistory(int userId)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<BookRental> BookHistory(int bookId)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<BookRental> Current()
    {
        throw new NotImplementedException();
    }

    public Task Loss(int rentId)
    {
        throw new NotImplementedException();
    }

    public BookRental Get(int id)
    {
        return _libApi.BookRentals.FirstOrDefault(i => i.Id == id);
    }

    public BookRental? GetExtended(int id)
    {
        throw new NotImplementedException();
    }

    public bool IsExists(int id)
    {
        throw new NotImplementedException();
    }
}