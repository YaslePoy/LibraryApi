using LibApi.Model;
using LibApi.Services.Base;

namespace LibApi.Services.RentService;

public interface IRentService : IExistable
{
    Task<int> TakeInRent(int userId, int bookCopyId, DateTime until);
    Task Return(int rentId);
    IReadOnlyList<BookRental> UserHistory(int userId);
    IReadOnlyList<BookRental> BookHistory(int bookId);
    IReadOnlyList<BookRental> Current();
    Task Loss(int rentId);
    BookRental Get(int id);
    BookRental? GetExtended(int id);
}