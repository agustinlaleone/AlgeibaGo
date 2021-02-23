using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlgeibaGoAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AlgeibaGoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, User")]
    public class RouteVisitsController : ControllerBase
    {
        private readonly AlgeibaGoContext _context;

        public RouteVisitsController(AlgeibaGoContext context)
        {
            _context = context;
        }

        // GET: api/RouteVisits
        [HttpGet]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public IEnumerable<RouteVisit> GetRouteVisit()
        {
            return _context.RouteVisit;
        }

        // GET: api/RouteVisits/5
        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetRouteVisit([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var routeVisit = await _context.RouteVisit.FindAsync(id);

            if (routeVisit == null)
            {
                return NotFound();
            }

            return Ok(routeVisit);
        }

        // PUT: api/RouteVisits/5
        [HttpPut("{id}")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> PutRouteVisit([FromRoute] int id, [FromBody] RouteVisit routeVisit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != routeVisit.Id)
            {
                return BadRequest();
            }

            _context.Entry(routeVisit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RouteVisitExists(id))
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

        // POST: api/RouteVisits
        [HttpPost]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> PostRouteVisit([FromBody] RouteVisit routeVisit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RouteVisit.Add(routeVisit);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRouteVisit", new { id = routeVisit.Id }, routeVisit);
        }

        // DELETE: api/RouteVisits/5
        [HttpDelete("{id}")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> DeleteRouteVisit([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var routeVisit = await _context.RouteVisit.FindAsync(id);
            if (routeVisit == null)
            {
                return NotFound();
            }

            _context.RouteVisit.Remove(routeVisit);
            await _context.SaveChangesAsync();

            return Ok(routeVisit);
        }

        private bool RouteVisitExists(int id)
        {
            return _context.RouteVisit.Any(e => e.Id == id);
        }
    }
}