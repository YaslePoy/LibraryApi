using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LibApi.DataBaseContext;
using LibApi.Model;
using LibApi.Requests;
using Microsoft.IdentityModel.Tokens;

namespace LibApi.Services.UserService;

public class UserService : IUserService
{
    private readonly UserApiContext _userApi;

    public UserService(UserApiContext userApi)
    {
        _userApi = userApi;
    }

    private const string Salt = "$2a$11$ZErcI.wI08ojlsW9Qcikle";

    public bool IsExists(int id)
    {
        return _userApi.Users.Any(i => i.Id == id);
    }

    public bool IsLoginExists(string login)
    {
        return _userApi.Users.Any(i => i.Login == login);
    }

    public IReadOnlyList<User> GetAll()
    {
        return _userApi.Users.ToList();
    }

    public IReadOnlyList<BookCopy> BooksOf(int id)
    {
        //Todo implement ms
        // return _libApi.BookCopies.Where(i => i.UserId == id).ToList();
        return null;
    }

    public string Authorize(string login, string password)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, Salt);
        var user = _userApi.Users.FirstOrDefault(i => i.Login == login && i.Password == passwordHash);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Authentication, user.Id.ToString()),
            new(ClaimTypes.Role, user.RoleId switch { 0 => "admin", _ => "reader" })
        };
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromHours(6)), // время действия 6 часов
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public async Task PlusBalance(int id, decimal sum)
    {
        var user = Get(id);
        if (user is null)
        
            return;
        user.Balance += sum;
        await _userApi.SaveChangesAsync();
    }

    public async Task<int> Create(CreateNewUser data)
    {
        var user = Utils.TransferData<User, CreateNewUser>(data);
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, Salt);
        await _userApi.Users.AddAsync(user);
        await _userApi.SaveChangesAsync();
        return user.Id;
    }

    public User? Get(int id)
    {
        return _userApi.Users.FirstOrDefault(i => i.Id == id);
    }

    public async Task Update(int id, UpdateUserRequest data)
    {
        var user = Get(data.Id);
        Utils.TransferData(user, data);
        await _userApi.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        _userApi.Users.Remove(Get(id));
        await _userApi.SaveChangesAsync();
    }
}