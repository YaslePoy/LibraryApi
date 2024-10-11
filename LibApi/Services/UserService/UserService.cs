using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LibApi.DataBaseContext;
using LibApi.Model;
using LibApi.Requests;
using Microsoft.IdentityModel.Tokens;

namespace LibApi.Services.UserService;

public class UserService : IUserService
{
    private readonly LibApiContext _libApi;
    private const string Salt = "$2a$11$ZErcI.wI08ojlsW9Qcikle";

    public bool IsExists(int id)
    {
        return _libApi.Users.Any(i => i.Id == id);
    }

    public bool IsLoginExists(string login)
    {
        return _libApi.Users.Any(i => i.Login == login);
    }

    public IReadOnlyList<User> GetAll()
    {
        return _libApi.Users.ToList();
    }

    public IReadOnlyList<BookCopy> BooksOf(int id)
    {
        return _libApi.BookCopies.Where(i => i.UserId == id).ToList();
    }

    public string Authorize(string login, string password)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, Salt);
        var user = _libApi.Users.FirstOrDefault(i => i.Login == login && i.Password == passwordHash);
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

    public async Task<int> Create(CreateNewUser data)
    {
        var user = Utils.TransferData<User, CreateNewUser>(data);
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, Salt);
        await _libApi.Users.AddAsync(user);
        await _libApi.SaveChangesAsync();
        return user.Id;
    }

    public User? Get(int id)
    {
        return _libApi.Users.FirstOrDefault(i => i.Id == id);
    }

    public async Task Update(int id, UpdateUserRequest data)
    {
        var user = Get(data.Id);
        Utils.TransferData(user, data);
        await _libApi.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        _libApi.Users.Remove(Get(id));
        await _libApi.SaveChangesAsync();
    }
}