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
using Microsoft.AspNetCore.JsonPatch;
using WebApiMacuco.Services;

namespace WebApiMacuco.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UserRecordsController : ControllerBase
  {
    private readonly ApplicationDbContext _context;
    private readonly AuthenticationService _authenticationService;

    public UserRecordsController(ApplicationDbContext context, IConfiguration configuration)
    {
      _context = context;
      _authenticationService = new AuthenticationService(configuration["Jwt:Secret"]);
    }

    [HttpPost("autenticar")]
    public ActionResult<AuthenticationResponse> Authenticate([FromBody] AuthenticationRequest request)
    {
      if (request.Username == "admin" && request.Password == "password")
      {
        var response = _authenticationService.GenerateJwtToken("1");
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

    // PUT: api/UserRecords/atualizar-face/5
    [HttpPut("atualizar-face/{id}")]
    [Authorize]
    public async Task<IActionResult> PutUserRecord(int id, UserRecord userRecord)
    {
      if (id != userRecord.UserId)
      {
        return BadRequest();
      }

      _context.Entry(userRecord).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!_context.UserRecords.Any(e => e.UserId == id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // PATCH: api/UserRecords/atualizar-face-parcial/5
    [HttpPatch("atualizar-face-parcial/{id}")]
    [Authorize]
    public async Task<IActionResult> PatchUserRecord(int id, [FromBody] JsonPatchDocument<UserRecord> patchDoc)
    {
      if (patchDoc == null)
      {
        return BadRequest();
      }

      var userRecord = await _context.UserRecords.FindAsync(id);
      if (userRecord == null)
      {
        return NotFound();
      }

      patchDoc.ApplyTo(userRecord, ModelState);

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      await _context.SaveChangesAsync();

      return Ok(userRecord);
    }

    // DELETE: api/UserRecords/excluir-face/5
    [HttpDelete("excluir-face/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUserRecord(int id)
    {
      var userRecord = await _context.UserRecords.FindAsync(id);
      if (userRecord == null)
      {
        return NotFound();
      }

      _context.UserRecords.Remove(userRecord);
      await _context.SaveChangesAsync();

      return NoContent();
    }
  }
}
