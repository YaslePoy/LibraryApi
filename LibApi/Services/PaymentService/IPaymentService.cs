namespace LibApi.Services;

public interface IPaymentService
{
    Task AccountReplenishment(int userId, decimal money);
}