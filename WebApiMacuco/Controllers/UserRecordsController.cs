using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiMacuco.Data;
using WebApiMacuco.Models;

namespace WebApiMacuco.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UserRecordsController : ControllerBase
  {
    private readonly ApplicationDbContext _context;
    private readonly string _jwtSecret;

    public UserRecordsController(ApplicationDbContext context, IConfiguration configuration)
    {
      _context = context;
      _jwtSecret = configuration["Jwt:Secret"];
    }

    // POST: api/UserRecords/autenticar
    [HttpPost("autenticar")]
    public ActionResult<AuthenticationResponse> Authenticate([FromBody] AuthenticationRequest request)
    {
      // Lógica de autenticação
      if (request.Username == "admin" && request.Password == "password")
      {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
          Subject = new ClaimsIdentity(new[] { new Claim("id", "1") }),
          Expires = DateTime.UtcNow.AddHours(1),
          SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        var response = new AuthenticationResponse
        {
          Token = tokenString,
          Expiration = tokenDescriptor.Expires.Value
        };
        return Ok(response);
      }

      return Unauthorized();
    }

    // POST: api/UserRecords/incluir-face
    [HttpPost("incluir-face")]
    [Authorize]
    public async Task<ActionResult<UserRecord>> PostUserRecord(UserRecord userRecord)
    {
      userRecord.CreatedAt = DateTime.UtcNow;
      _context.UserRecords.Add(userRecord);
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetUserRecord", new { id = userRecord.UserId }, userRecord);
    }

    // GET: api/UserRecords/listar-todas-as-faces
    [HttpGet("listar-todas-as-faces")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserRecord>>> GetUserRecords()
    {
      return await _context.UserRecords.ToListAsync();
    }

    // GET: api/UserRecords/listar-faces-por-gestor-empresa
    [HttpGet("listar-faces-por-gestor-empresa")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserRecord>>> GetUserRecordsByManager(string gestor)
    {
      return await _context.UserRecords
                           .Where(r => r.UserCode == gestor)
                           .ToListAsync();
    }

    // POST: api/UserRecords/match-faces
    [HttpPost("match-faces")]
    [Authorize]
    public ActionResult<bool> MatchFaces([FromBody] string faceTemplate)
    {
      // Lógica de matching de faces
      return Ok(true); // Retorna true como exemplo
    }
  }
}
