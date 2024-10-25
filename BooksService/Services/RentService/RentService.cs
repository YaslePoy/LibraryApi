using LibApi.DataBaseContext;
using LibApi.Model;
using LibApi.Services.BookService;
using LibApi.Services.UserService;
using Microsoft.EntityFrameworkCore;

namespace LibApi.Services.RentService;

public class RentService : IRentService
{
    private readonly BooksApiContext _booksApi;
    private readonly IUserService _user;
    private readonly IBookService _book;

    public RentService(BooksApiContext booksApi, IUserService user, IBookService book)
    {
        _booksApi = booksApi;
        _user = user;
        _book = book;
    }

    public async Task<int> TakeInRent(int userId, int bookCopyId, DateTime until)
    {
        var tariff = _booksApi.Tariffs.FirstOrDefault(i => i.IsActive);

        var transaction = new Transaction
        {
            UserId = userId,
            Movement = -(tariff.PaymentPerDay * (decimal)((until - DateTime.Now).TotalHours / 24)),
            TransactionTime = DateTime.Now
        };
        var user = _user.Get(userId);

        var currentRents = _booksApi.BookRentals.Include(i => i.BookCopy)
            .Where(i => i.IsReturned == false && i.Payment.UserId == userId).ToList();

        var totalDeposit = currentRents.Sum(i => i.BookCopy.Cost);
        var book = _book.GetCopy(bookCopyId);
        if (user.Balance - totalDeposit - book.Cost < 0)
            throw new Exception("Need money for deposit and rent");

        await _booksApi.Transactions.AddAsync(transaction);
        await _booksApi.SaveChangesAsync();

        var rent = new BookRental
        {
            TariffId = tariff.Id,
            BookCopyId = bookCopyId,
            RentStart = DateTime.Now,
            RentEnd = until,
            PaymentId = transaction.Id
        };

        book.UserId = userId;
        await _booksApi.BookRentals.AddAsync(rent);
        await _booksApi.SaveChangesAsync();
        _user.PlusBalance(userId, transaction.Movement);
        return rent.Id;
    }

    public async Task Return(int rentId)
    {
        var rent = _booksApi.BookRentals.Include(i => i.BookCopy).FirstOrDefault(i => i.Id == rentId);
        rent.IsReturned = true;
        rent.BookCopy.UserId = null;

        await _booksApi.SaveChangesAsync();
    }

    public IReadOnlyList<BookRental> UserHistory(int userId)
    {
        return _booksApi.BookRentals.Where(i => i.Payment.UserId == userId).ToList();
    }

    public IReadOnlyList<BookRental> BookHistory(int bookId)
    {
        return _booksApi.BookRentals.Where(i => i.BookCopyId == bookId).ToList();
    }

    public IReadOnlyList<BookRental> Current()
    {
        return _booksApi.BookRentals.Where(i => i.IsReturned == false).Include(i => i.BookCopy)
            .Include(i => i.BookCopy.Book).ToList();
    }

    public async Task Loss(int rentId)
    {
        var rent = GetExtended(rentId);
        rent.BookCopy.IsLost = true;
        _user.PlusBalance(rent.Payment.UserId, -rent.BookCopy.Cost);
        var transaction = new Transaction
        {
            UserId = rent.Payment.UserId,
            Movement = -rent.BookCopy.Cost,
            TransactionTime = DateTime.Now
        };
        await _booksApi.Transactions.AddAsync(transaction);
        await _booksApi.SaveChangesAsync();
    }

    public BookRental Get(int id)
    {
        return _booksApi.BookRentals.FirstOrDefault(i => i.Id == id);
    }

    public BookRental? GetExtended(int id)
    {
        return _booksApi.BookRentals.Include(i => i.BookCopy).FirstOrDefault(i => i.Id == id);
    }

    public bool IsExists(int id)
    {
        return _booksApi.BookRentals.Any(i => i.Id == id);
    }
}