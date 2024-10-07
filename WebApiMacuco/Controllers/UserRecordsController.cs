using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Adapters;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiMacuco.Data;
using WebApiMacuco.Models;
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
            if (request.Username == "administrator" && request.Password == "ewV4SG%aKLRXzh")
            {
                var response = _authenticationService.GenerateJwtToken("1");
                return Ok(response);
            }

            return Unauthorized();
        }

        [HttpGet("{id}")]
        public ActionResult<UserRecord> GetUserRecord(int id)
        {
            var userRecord = _context.UserRecords.Find(id);
            if (userRecord == null)
            {
                return NotFound();
            }
            return Ok(userRecord);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateUserRecord(int id, [FromBody] JsonPatchDocument<UserRecord> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var userRecord = _context.UserRecords.Find(id);
            if (userRecord == null)
            {
                return NotFound();
            }

            // Adaptar a ação de log de erro para adicionar ao ModelState
            void LogError(JsonPatchError error)
            {
                ModelState.AddModelError("JsonPatchError", error.ErrorMessage);
            }

            try
            {
                patchDoc.ApplyTo(userRecord, LogError);
            }
            catch (JsonPatchException ex)
            {
                ModelState.AddModelError("JsonPatchError", ex.Message);
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SaveChanges();

            return NoContent();
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

            // Adaptar a ação de log de erro para adicionar ao ModelState
            void LogError(JsonPatchError error)
            {
                ModelState.AddModelError("JsonPatchError", error.ErrorMessage);
            }

            try
            {
                patchDoc.ApplyTo(userRecord, LogError);
            }
            catch (JsonPatchException ex)
            {
                ModelState.AddModelError("JsonPatchError", ex.Message);
                return BadRequest(ModelState);
            }

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
