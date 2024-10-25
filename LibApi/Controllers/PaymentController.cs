using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LibApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LibApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : CheckController
{
    private const string PaypentPath = "http://localhost:5187/api/Payment/";
    public static readonly string PaymentPage = System.IO.File.ReadAllText("payment.html");

    [HttpGet("replenishment")]
    public IActionResult GetPaymentPage(int userId, decimal money)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Authentication, userId.ToString()),
            new(ClaimTypes.Role, "payment"),
            new("payment_id", Guid.NewGuid().ToString())
        };
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)), // время действия 5 минут
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        
        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return base.Content(
            PaymentPage.Replace("const jwtoken = ''", $"const jwtoken = '{token}'")
                .Replace("const link = ''",
                    $"const link = 'http://192.168.1.68:5259/api/Payment/replenishment?userId={userId}&money={money}'")
                .Replace("Оплатить", $"Оплатить {Math.Round((double)money, 2)}"), "text/html");
        // return Redirect("https://microsoft.com");
    }

    [Authorize(Roles = "payment")]
    [HttpPost("replenishment")]
    public async Task<ActionResult> AccountReplenishment(int userId, decimal money)
    {
        using var http = new HttpClient();

        return Ok((await http.PostAsync($"{PaypentPath}replenishment?userId={userId}&money={money}", new StringContent(""))).StatusCode);
    }

    [HttpGet("history/{userId}")]
    [Authorize]
    public async Task<ActionResult> GetHistoryByUser(int userId)
    {
        if (ChechFromJWT(ClaimTypes.Authentication, userId.ToString()) &&
            ChechFromJWT(ClaimTypes.Role, "admin"))
            return Unauthorized("User can see history of trasactions only for his account");
        using var http = new HttpClient();

        return Ok(http.GetStringAsync($"{PaypentPath}history/{userId}"));
    }
}