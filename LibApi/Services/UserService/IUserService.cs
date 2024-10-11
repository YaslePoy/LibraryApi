using LibApi.Model;
using LibApi.Requests;
using LibApi.Services.Base;

namespace LibApi.Services.UserService;

public interface IUserService : ICrudService<User, CreateNewUser, UpdateUserRequest>
{
    bool IsLoginExists(string login);
    IReadOnlyList<User> GetAll();
    IReadOnlyList<BookCopy> BooksOf(int id);
    string Authorize(string login, string password);
}