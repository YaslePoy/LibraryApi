using System.Text;
using Microsoft.EntityFrameworkCore;
using LibApi.DataBaseContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.ISSUER,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed((host) => true)
            .AllowAnyHeader());
});
builder.Services.AddDbContext<LibApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")), ServiceLifetime.Scoped);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();

public class AuthOptions
{
    public const string ISSUER = "LibAuthServer"; // издатель токена
    public const string AUDIENCE = "Client"; // потребитель токена
    const string KEY = "5F7652F2-19DE-4954-B5D4-BC7B92F95C9E"; // ключ для шифрации

    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}


// var builder = WebApplication.CreateBuilder();
//  
// builder.Services.AddAuthorization();
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             // указывает, будет ли валидироваться издатель при валидации токена
//             ValidateIssuer = true,
//             // строка, представляющая издателя
//             ValidIssuer = AuthOptions.ISSUER,
//             // будет ли валидироваться потребитель токена
//             ValidateAudience = true,
//             // установка потребителя токена
//             ValidAudience = AuthOptions.AUDIENCE,
//             // будет ли валидироваться время существования
//             ValidateLifetime = true,
//             // установка ключа безопасности
//             IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
//             // валидация ключа безопасности
//             ValidateIssuerSigningKey = true,
//          };
// });
//
//
// var app = builder.Build();
//  
// app.UseAuthentication();
// app.UseAuthorization();
//  
// app.Map("/login/{username}", (string username) => 
// {
//     var claims = new List<Claim> {new Claim(ClaimTypes.Name, username) };
//     // создаем JWT-токен
//     var jwt = new JwtSecurityToken(
//             issuer: AuthOptions.ISSUER,
//             audience: AuthOptions.AUDIENCE,
//             claims: claims,
//             expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
//             signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
//             
//     return new JwtSecurityTokenHandler().WriteToken(jwt);
// });
//  
// app.Map("/data", [Authorize] () => new { message= "Hello World!" });
//  
// app.Run();
//  
// public class AuthOptions
// {
//     public const string ISSUER = "MyAuthServer"; // издатель токена
//     public const string AUDIENCE = "MyAuthClient"; // потребитель токена
//     const string KEY = "mysupersecret_secretsecretsecretkey!123";   // ключ для шифрации
//     public static SymmetricSecurityKey GetSymmetricSecurityKey() => 
//         new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
// }