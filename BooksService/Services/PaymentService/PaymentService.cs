using LibApi.DataBaseContext;
using LibApi.Model;
using LibApi.Services.UserService;

namespace LibApi.Services;

public class PaymentService : IPaymentService
{
    private readonly IUserService _user;
    private readonly BooksApiContext _booksApi;

    public PaymentService(BooksApiContext booksApi)
    {
        _booksApi = booksApi;
    }

    public async Task AccountReplenishment(int userId, decimal money)
    {
        _user.PlusBalance(userId, money);
        var transaction = new Transaction
        {
            UserId = userId,
            Movement = money,
            TransactionTime = DateTime.Now
        };

        await _booksApi.Transactions.AddAsync(transaction);
        await _booksApi.SaveChangesAsync();
    }
}