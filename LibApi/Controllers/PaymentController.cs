using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LibApi.DataBaseContext;
using LibApi.Model;
using LibApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LibApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : CheckController
{
    public static readonly string PaymentPage = System.IO.File.ReadAllText("payment.html");

    public static readonly HashSet<string> PaymentIds = new HashSet<string>();

    private readonly LibApiContext _libApi;
    private readonly IPaymentService _paymentService;

    public PaymentController(LibApiContext libApi, IPaymentService paymentService)
    {
        _libApi = libApi;
        _paymentService = paymentService;
    }

    [HttpGet("replenishment")]
    public ContentResult GetPaymentPage(int userId, decimal money)
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
    }

    [Authorize(Roles = "payment")]
    [HttpPost("replenishment")]
    public async Task<ActionResult> AccountReplenishment(int userId, decimal money)
    {
        var tokenStr = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenStr);
        var paymentId = token.Claims.FirstOrDefault(i => i.Type == "payment_id").Value;

        if (PaymentIds.Contains(paymentId))
            return BadRequest("This payment is not active");

        PaymentIds.Add(paymentId);
        var user = _libApi.Users.FirstOrDefault(i => i.Id == userId);

        if (user is null)
            return NotFound($"No user with id {userId}");

        await _paymentService.AccountReplenishment(userId, money);

        return Ok();
    }

    [HttpGet("history/{userId}")]
    [Authorize]
    public ActionResult GetHistoryByUser(int userId)
    {
        if (ChechFromJWT(ClaimTypes.Authentication, userId.ToString()) &&
            ChechFromJWT(ClaimTypes.Role, "admin"))
            return Unauthorized("User can see history of trasactions only for his account");
        return Ok(_libApi.Transactions.Where(i => i.UserId == userId).ToList());
    }
}