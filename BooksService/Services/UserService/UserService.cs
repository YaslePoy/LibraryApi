using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using LibApi.DataBaseContext;
using LibApi.Model;
using LibApi.Requests;
using Microsoft.IdentityModel.Tokens;

namespace LibApi.Services.UserService;

public class UserService : IUserService
{
    private const string UsersPath = "http://localhost:5178/api/";
    private readonly BooksApiContext _booksApi;

    public Task<int> Create(CreateNewUser data)
    {
        throw new NotImplementedException();
    }

    public User Get(int id)
    {
        //TODO implement microservice


        using var http = new HttpClient();
        var responseTask = http.GetAsync($"{UsersPath}Users/{id}");
        responseTask.Wait();
        var response = responseTask.Result;
        if (!response.IsSuccessStatusCode)
            return null;
        var responseContent = response.Content.ReadAsStringAsync();
        responseContent.Wait();
        return JsonSerializer.Deserialize<User>(responseContent.Result);
    }

    public Task Update(int id, UpdateUserRequest data)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void PlusBalance(int id, decimal sum)
    {
        using var http = new HttpClient();
       http.PostAsync($"{UsersPath}Users/balance?id={id}&sum={sum}", new StringContent(""));
    }

    public bool IsExists(int id)
    {
        throw new NotImplementedException();
    }
}