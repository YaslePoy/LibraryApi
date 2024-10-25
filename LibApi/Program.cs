using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProxyKit;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
//         };
//     });

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

builder.Services.AddProxy();
var app = builder.Build();

// app.UseAuthentication();
// app.UseAuthorization();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWhen(context => context.Request.Path.Value.Contains("/api/Books"),
    applicationBuilder => applicationBuilder.RunProxy(context =>
        context.ForwardTo("http://localhost:5187").AddXForwardedHeaders().Send()));
app.UseWhen(context => context.Request.Path.Value.Contains("/api/Genres"),
    applicationBuilder => applicationBuilder.RunProxy(context =>
        context.ForwardTo("http://localhost:5187").AddXForwardedHeaders().Send()));
app.UseWhen(context => context.Request.Path.Value.Contains("/api/Payment"),
    applicationBuilder => applicationBuilder.RunProxy(context =>
        context.ForwardTo("http://localhost:5187").AddXForwardedHeaders().Send()));
app.UseWhen(context => context.Request.Path.Value.Contains("/api/Rent"),
    applicationBuilder => applicationBuilder.RunProxy(context =>
        context.ForwardTo("http://localhost:5187").AddXForwardedHeaders().Send()));
app.UseWhen(context => context.Request.Path.Value.Contains("/api/Users"),
    applicationBuilder => applicationBuilder.RunProxy(context =>
        context.ForwardTo("http://localhost:5178").AddXForwardedHeaders().Send()));

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