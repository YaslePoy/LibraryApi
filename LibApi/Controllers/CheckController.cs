﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;

namespace LibApi.Controllers;

public class CheckController : Controller
{
    public bool ChechFromJWT(string key, string value)
    {
        var tokenStr = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(tokenStr);

        var claim = token.Claims.FirstOrDefault(i => i.Type == key).Value;
        return claim != value;
    }
}