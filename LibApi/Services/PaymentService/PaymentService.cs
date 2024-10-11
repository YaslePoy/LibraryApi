using LibApi.DataBaseContext;
using LibApi.Model;

namespace LibApi.Services;

public class PaymentService : IPaymentService
{
    private readonly LibApiContext _libApi;

    public PaymentService(LibApiContext libApi)
    {
        _libApi = libApi;
    }

    public async Task AccountReplenishment(int userId, decimal money)
    {
        var user = _libApi.Users.FirstOrDefault(i => i.Id == userId);
        user.Balance += money;

        var transaction = new Transaction
        {
            UserId = userId,
            Movement = money,
            TransactionTime = DateTime.Now
        };

        await _libApi.Transactions.AddAsync(transaction);
        await _libApi.SaveChangesAsync();
    }
}