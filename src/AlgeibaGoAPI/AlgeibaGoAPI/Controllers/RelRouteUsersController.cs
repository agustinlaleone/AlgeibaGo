using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlgeibaGoAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;

namespace AlgeibaGoAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, User")]
    public class RelRouteUsersController : ControllerBase
    {
        private readonly AlgeibaGoContext _context;

        public RelRouteUsersController(AlgeibaGoContext context)
        {
            _context = context;
        }

        // GET: api/RelRouteUsers
        [HttpGet]
        public IEnumerable<RelRouteUser> GetRelRouteUser()
        {
            return _context.RelRouteUser;
        }

        // GET: api/RelRouteUsers/5
        [HttpGet("{id}")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetRelRouteUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var relRouteUser = await _context.RelRouteUser.FindAsync(id);

            if (relRouteUser == null)
            {
                return NotFound();
            }

            return Ok(relRouteUser);
        }

        // PUT: api/RelRouteUsers/5
        [HttpPut("{id}")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> PutRelRouteUser([FromRoute] int id, [FromBody] RelRouteUser relRouteUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != relRouteUser.IdRelation)
            {
                return BadRequest();
            }

            _context.Entry(relRouteUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RelRouteUserExists(id))
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

        // POST: api/RelRouteUsers
        [HttpPost]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> PostRelRouteUser([FromBody] RelRouteUser relRouteUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RelRouteUser.Add(relRouteUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRelRouteUser", new { id = relRouteUser.IdRelation }, relRouteUser);
        }

        // DELETE: api/RelRouteUsers/5
        [HttpDelete("{id}")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> DeleteRelRouteUser([FromRoute] int id)
        {
            try
            {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<RelRouteUser> relRouteUsers = _context.RelRouteUser.Where(x => x.IdRoute == id).ToList();

            if (relRouteUsers == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var rels in relRouteUsers)
                {
                    _context.RelRouteUser.Remove(rels);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(relRouteUsers);

            }catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private bool RelRouteUserExists(int id)
        {
            return _context.RelRouteUser.Any(e => e.IdRelation == id);
        }

        // Post: api/RelRouteUsers/SaveRelations
        [HttpPost("SaveRelations")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult> SaveRelations(UsersRoute usersRoutes)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<RelRouteUser> relRouteUsers = _context.RelRouteUser.Where(x => x.IdRoute == usersRoutes.idRoute).ToList();

            if (relRouteUsers == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var rels in relRouteUsers)
                {
                    _context.RelRouteUser.Remove(rels); ;
                }
            }

            foreach (var user in usersRoutes.relations)
            {
                var rel = new RelRouteUser { IdRoute = usersRoutes.idRoute, IdUser = user.Id };
                _context.RelRouteUser.Add(rel);
            }

            await _context.SaveChangesAsync();

            return Ok();

        }
    }
}