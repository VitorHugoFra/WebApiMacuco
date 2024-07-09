using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiMacuco.Models;

namespace WebApiMacuco.Services
{
  public class AuthenticationService
  {
    private readonly string _jwtSecret;

    public AuthenticationService(string jwtSecret)
    {
      _jwtSecret = jwtSecret;
    }

    public AuthenticationResponse GenerateJwtToken(string userId)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(_jwtSecret);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new[] { new Claim("id", userId) }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };
      var token = tokenHandler.CreateToken(tokenDescriptor);
      var tokenString = tokenHandler.WriteToken(token);

      return new AuthenticationResponse
      {
        Token = tokenString,
        Expiration = tokenDescriptor.Expires.Value
      };
    }
  }
}
