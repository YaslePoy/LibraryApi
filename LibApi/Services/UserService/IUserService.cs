using LibApi.Model;
using LibApi.Requests;
using LibApi.Services.Base;

namespace LibApi.Services.UserService;

public interface IUserService : ICrudService<User, CreateNewUser, UpdateUserRequest>
{
}