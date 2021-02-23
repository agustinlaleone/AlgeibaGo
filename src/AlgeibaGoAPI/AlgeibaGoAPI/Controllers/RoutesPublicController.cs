using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AlgeibaGoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlgeibaGoAPI.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class RoutesPublicController : Controller
    {
        private readonly AlgeibaGoContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        public RoutesPublicController(AlgeibaGoContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("registerRouteVisit")]
        [MapToApiVersion("1.0")]
        public IActionResult RegisterRouteVisit([FromBody]int id)
        {
            try
            {
                if (_context.Routes.FindAsync(id) == null)
                    return NotFound();

                RouteVisit visit = new RouteVisit();
                visit.RouteId = id;
                visit.VisitDate = DateTime.Now;
                _context.RouteVisit.Add(visit);
                _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }

        [HttpPost("registerRouteVisit")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> RegisterRouteVisit([FromBody]RouteVisit routevisit)
        {
            try
            {
                if (_context.Routes.Any(p => p.Id == routevisit.Id)) return NotFound();

                RouteVisit visit = new RouteVisit();
                visit.RouteId = routevisit.RouteId;
                visit.VisitDate = DateTime.Now;
                visit.Browser = routevisit.Browser;
                visit.BrowserVersion = routevisit.BrowserVersion;
                visit.Device = routevisit.Device;
                visit.OS = routevisit.OS;
                visit.OSVersion = routevisit.OSVersion;
                visit.UserAgent = routevisit.UserAgent;
                visit.IPData = routevisit.IPData;
                visit.Country = routevisit.Country;
                _context.RouteVisit.Add(visit);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("GetByName/{name}")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetRouteByString([FromRoute] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var routes = await _context.Routes.Include("RouteVisit").Include("FavoriteRoute").Where(o => o.Route.ToLower() == name.ToLower()).FirstOrDefaultAsync();
            if (routes == null || !routes.Status.Value)
            {
                return NotFound();
            }
            return Ok(routes);
        }
    }
}