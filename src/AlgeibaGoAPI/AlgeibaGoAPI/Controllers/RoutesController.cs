using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlgeibaGoAPI.Models;
using AlgeibaGoAPI.Extensions;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using System.Text.RegularExpressions;

namespace AlgeibaGoAPI.Controllers
{
    public class PagingParameters
    {
        public int pageIndex;
        public int pageSize;
        public string filter;
        public string sortOrder;
        public bool sortAsc { get { return sortOrder != null ? (sortOrder.ToLower() == "asc") : false; } }
        public string user;
        public string role;
    }

    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, User")]
    public class RoutesController : ControllerBase
    {
        private readonly AlgeibaGoContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public RoutesController(AlgeibaGoContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/Routes
        [HttpGet]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public IEnumerable<UsersByRoutes> GetRoutes()
        {
            List<UsersByRoutes> usersByRoutes = new List<UsersByRoutes>();
            var routes = _context.Routes.ToList();
            foreach (var item in routes)
            {
                var usersByRoutesQuery = (from rr in _context.RelRouteUser
                                          join u in _context.AspNetUsers on rr.IdUser equals u.Id
                                          where rr.IdRoute == item.Id
                                          select new User { Id = rr.IdUser, UserName = u.UserName }).ToList();
                var usersByRoutesModel = new UsersByRoutes { Id = item.Id, Users = usersByRoutesQuery, RedirectUrl = item.RedirectUrl, Route = item.Route, RelRouteUser = item.RelRouteUser, RouteVisit = item.RouteVisit, Status = item.Status };
                usersByRoutes.Add(usersByRoutesModel);
            }
            return usersByRoutes;
        }

        // GET: api/Routes/5
        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetRoutes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var routes = await _context.Routes.FindAsync(id);

            if (routes == null)
            {
                return NotFound();
            }

            return Ok(routes);
        }


        // GET: api/Routes/GetPaged/{param}
        [HttpPost("GetPaged")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public IEnumerable<UsersByRoutes> GetPaged(PagingParameters param)
        {
            if (_context.Routes == null || _context.Routes.Count() == 0) return null;

            IQueryable<Routes> routes = _context.Routes.Include("RouteVisit").Where(o => !string.IsNullOrEmpty(param.filter) ? o.Route.StartsWith(param.filter) : true).Skip(param.pageIndex * param.pageSize).Take(param.pageSize);
            List<UsersByRoutes> usersByRoutes = new List<UsersByRoutes>();
            try
            {


                foreach (var item in routes)
                {
                    var usersByRoute = (from rr in _context.RelRouteUser
                                        join u in _context.AspNetUsers on rr.IdUser equals u.Id
                                        where rr.IdRoute == item.Id
                                        select new User { Id = rr.IdUser, UserName = u.UserName }).ToList();

                    var usersByRoutesModel = new UsersByRoutes { Id = item.Id, PageTitle = item.PageTitle, isFavorite = null, Users = usersByRoute, RedirectUrl = item.RedirectUrl, Route = item.Route, RelRouteUser = item.RelRouteUser, RouteVisit = item.RouteVisit, Status = item.Status };

                    if (param.role == "Admin")
                    {
                        usersByRoutesModel.Exists = true;
                        usersByRoutes.Add(usersByRoutesModel);
                    }
                    else
                    {

                        if (usersByRoutesModel.Users.Exists(x => x.Id == param.user))
                        {
                            usersByRoutesModel.Exists = true;
                        }
                        else
                        {
                            usersByRoutesModel.Exists = false;
                        }
                        usersByRoutes.Add(usersByRoutesModel);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            var usersByRoutesQuery = usersByRoutes.AsQueryable();
            if (!string.IsNullOrEmpty(param.sortOrder))
            {
                if (param.sortOrder == "VisitCount")
                {
                    if (param.sortAsc)
                        usersByRoutesQuery = usersByRoutesQuery.OrderBy(x => x.RouteVisit.Count);
                    else
                        usersByRoutesQuery = usersByRoutesQuery.OrderByDescending(x => x.RouteVisit.Count);
                }
            }

            return usersByRoutesQuery;
        }

        // GET: api/Routes/GetOwnPaged/{param}
        [HttpPost("GetOwnPaged")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public IEnumerable<UsersByRoutes> GetOwnPaged(PagingParameters param)
        {
            if (_context.Routes == null || _context.Routes.Count() == 0) return null;

            var routesByUser = (from r in _context.Routes.Include("RouteVisit").Include("RelRouteUser")
                                join rr in _context.RelRouteUser on r.Id equals rr.IdRoute
                                where rr.IdUser == param.user
                                select r).Skip(param.pageIndex * param.pageSize).Take(param.pageSize).ToList();

            List<UsersByRoutes> usersByRoutes = new List<UsersByRoutes>();

            foreach (var item in routesByUser)
            {
                var flagUser = false;
                var usersByRoute = (from rr in _context.RelRouteUser
                                    join u in _context.AspNetUsers on rr.IdUser equals u.Id
                                    where rr.IdRoute == item.Id
                                    select new User { Id = rr.IdUser, UserName = u.UserName }).ToList();
                var favoritesUserFlag = _context.Favorites.Where(x => x.IdUser == param.user && item.Id == x.IdRoute).FirstOrDefault();

                if (favoritesUserFlag != null)
                    flagUser = favoritesUserFlag.isFavorite;
                var usersByRoutesModel = new UsersByRoutes { Id = item.Id, PageTitle = item.PageTitle, isFavorite = flagUser, Users = usersByRoute, RedirectUrl = item.RedirectUrl, Route = item.Route, RelRouteUser = item.RelRouteUser, RouteVisit = item.RouteVisit, Status = item.Status };
                usersByRoutesModel.Exists = true;
                usersByRoutes.Add(usersByRoutesModel);
            }
            var usersByRoutesQuery = usersByRoutes.AsQueryable();
            if (!string.IsNullOrEmpty(param.sortOrder))
            {
                if (param.sortOrder == "VisitCount")
                {
                    if (param.sortAsc)
                        usersByRoutesQuery = usersByRoutesQuery.OrderBy(x => x.RouteVisit.Count);
                    else
                        usersByRoutesQuery = usersByRoutesQuery.OrderByDescending(x => x.RouteVisit.Count);
                }
            }

            return usersByRoutes;
        }


        [HttpPost("GetMyFavoritesLink")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public IEnumerable<UsersByRoutes> GetMyFavoritesLink(PagingParameters param)
        {
            if (_context.Routes == null || _context.Routes.Count() == 0) return null;

            var routesByUser = (from r in _context.Routes.Include("RouteVisit").Include("RelRouteUser")
                                join rr in _context.Favorites on r.Id equals rr.IdRoute
                                where rr.IdUser == param.user && rr.isFavorite == true
                                select r).Skip(param.pageIndex * param.pageSize).Take(param.pageSize).ToList();
            List<UsersByRoutes> usersByRoutes = new List<UsersByRoutes>();
            foreach (var item in routesByUser)
            {
                var flagUser = false;
                var usersByRoute = (from rr in _context.Favorites
                                    join u in _context.AspNetUsers on rr.IdUser equals u.Id
                                    where rr.IdRoute == item.Id
                                    select new User { Id = rr.IdUser, UserName = u.UserName }).ToList();
                var favoritesUserFlag = _context.Favorites.Where(x => x.IdUser == param.user && item.Id == x.IdRoute).FirstOrDefault();

                if (favoritesUserFlag != null)
                    flagUser = favoritesUserFlag.isFavorite;
                var usersByRoutesModel = new UsersByRoutes { Id = item.Id, PageTitle = item.PageTitle, isFavorite = flagUser, Users = usersByRoute, RedirectUrl = item.RedirectUrl, Route = item.Route, RelRouteUser = item.RelRouteUser, RouteVisit = item.RouteVisit, Status = item.Status };
                usersByRoutesModel.Exists = true;
                usersByRoutes.Add(usersByRoutesModel);
            }
            var usersByRoutesQuery = usersByRoutes.AsQueryable().OrderByDescending(x => x.VisitCount);
            if (!string.IsNullOrEmpty(param.sortOrder))
            {
                if (param.sortOrder == "VisitCount")
                {
                    if (param.sortAsc)
                        usersByRoutesQuery = usersByRoutesQuery.OrderBy(x => x.RouteVisit.Count);
                    else
                        usersByRoutesQuery = usersByRoutesQuery.OrderByDescending(x => x.RouteVisit.Count);
                }
            }

            return usersByRoutes.OrderByDescending(x => x.VisitCount);
        }
        // GET: api/Routes/GetOthersPaged/{param}
        [HttpPost("GetOthersPaged")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public IEnumerable<UsersByRoutes> GetOthersPaged(PagingParameters param)
        {
            if (_context.Routes == null || _context.Routes.Count() == 0) return null;

            IQueryable<Routes> routes = _context.Routes.Include("RouteVisit").Where(o => o.Route.StartsWith(param.filter.ToString())).Skip(param.pageIndex * param.pageSize).Take(param.pageSize);
            List<UsersByRoutes> usersByRoutes = new List<UsersByRoutes>();
            foreach (var item in routes)
            {
                var flagUser = false;

                var usersByRoute = (from rr in _context.RelRouteUser
                                    join u in _context.AspNetUsers on rr.IdUser equals u.Id
                                    where rr.IdRoute == item.Id
                                    select new User { Id = rr.IdUser, UserName = u.UserName }).ToList();
                var favoritesUserFlag = _context.Favorites.Where(x => x.IdUser == param.user && item.Id == x.IdRoute).FirstOrDefault();
                if (favoritesUserFlag != null)
                    flagUser = favoritesUserFlag.isFavorite;
                var usersByRoutesModel = new UsersByRoutes { Id = item.Id, PageTitle = item.PageTitle, isFavorite = flagUser, Users = usersByRoute, RedirectUrl = item.RedirectUrl, Route = item.Route, RelRouteUser = item.RelRouteUser, RouteVisit = item.RouteVisit, Status = item.Status };

                if (!usersByRoutesModel.Users.Exists(x => x.Id == param.user))
                {
                    if (param.role == "Admin")
                    {
                        usersByRoutesModel.Exists = true;
                    }
                    else
                    {
                        usersByRoutesModel.Exists = false;
                    }
                    usersByRoutes.Add(usersByRoutesModel);
                }


            }
            var usersByRoutesQuery = usersByRoutes.AsQueryable();
            if (!string.IsNullOrEmpty(param.sortOrder))
            {
                if (param.sortOrder == "VisitCount")
                {
                    if (param.sortAsc)
                        usersByRoutesQuery = usersByRoutesQuery.OrderBy(x => x.RouteVisit.Count);
                    else
                        usersByRoutesQuery = usersByRoutesQuery.OrderByDescending(x => x.RouteVisit.Count);
                }
            }

            return usersByRoutesQuery;
        }

        [HttpPost("GetCount")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public long GetCount(PagingParameters param)
        {
            if (_context.Routes == null || _context.Routes.Count() == 0) return 0;

            return _context.Routes.Where(o => o.Route.StartsWith(param.filter.ToString())).Count();
        }

        [HttpPost("GetOwnCount")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public long GetOwnCount(PagingParameters param)
        {
            if (_context.Routes == null || _context.Routes.Count() == 0) return 0;

            var count = _context.RelRouteUser.Where(x => x.IdUser == param.user).Count();

            return count;
        }

        [HttpPost("GetOthersCount")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public long GetOthersCount(PagingParameters param)
        {
            if (_context.Routes == null || _context.Routes.Count() == 0) return 0;

            var count = GetCount(param);
            var own = GetOwnCount(param);
            count = count - own;

            return count;
        }

        // PUT: api/Routes
        [HttpPut]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> PutRoutes([FromBody] Routes routes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (routes == null || routes.Id == 0)
            {
                return BadRequest();
            }

            Boolean exist = _context.Routes.Where(o => o.Id != routes.Id && o.Route.ToLower() == routes.Route.ToLower()).Any();
            if (exist)
                return BadRequest("La ruta ya se encuentra en uso!");

            _context.Entry(routes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoutesExists(routes.Id))
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

        // POST: api/Routes
        [HttpPost]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> PostRoutes([FromBody] Routes routes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Routes.Where(o => o.Route.ToLower() == routes.Route.ToLower()).Any())
                return BadRequest("La ruta ya se encuentra en uso!");

            WebClient wc = new WebClient();

            string html = "";
            string titulo = "";
            try
            {
                html = wc.DownloadString(routes.RedirectUrl);
                Regex x = new Regex("<title>(.*)</title>");

                MatchCollection m = x.Matches(html);
                if (m.Count > 0)
                {
                    titulo = m[0].Value.Replace("<title>", "").Replace("</title>", "");
                }
            }
            catch
            {
            }

            routes.PageTitle = titulo;
            _context.Routes.Add(routes);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoutes", new { id = routes.Id }, routes);
        }

        // DELETE: api/Routes/5
        [HttpDelete("{id}")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> DeleteRoutes([FromRoute] int id)
        {
            try
            {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var routes = await _context.Routes.Include("RouteVisit").FirstOrDefaultAsync(x => x.Id == id);
            if (routes == null)
            {
                return NotFound();
            }

            if (routes.RouteVisit != null && routes.RouteVisit.Count > 0)
                _context.RouteVisit.RemoveRange(routes.RouteVisit);

            _context.Routes.Remove(routes);
            await _context.SaveChangesAsync();

            return Ok(routes);

            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private bool RoutesExists(int id)
        {
            return _context.Routes.Any(e => e.Id == id);
        }
        private partial class Percentage
        {
            public int y;
            public string name;
            public Percentage()
            {
                this.y = 0;
                this.name = "null";
            }
        }

        [HttpGet("GetStatiticsCountry")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetStatiticsCountry(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var routes = await _context.Routes.Include("RouteVisit").FirstOrDefaultAsync(x => x.Id == id);
            if (routes == null)
            {
                return NotFound();
            }
            List<Percentage> routevisits = new List<Percentage>();
            foreach (var item in routes.RouteVisit)
            {
                var isThere = routevisits.Where(x => x.name == item.Country).FirstOrDefault();
                if (isThere != null)
                {
                    var index = routevisits.FindIndex(x => x.name == item.Country);
                    routevisits[index].y = routevisits[index].y + 1;
                }
                else
                {
                    Percentage addToList = new Percentage();
                    addToList.name = item.Country;
                    addToList.y = 1;
                    routevisits.Add(addToList);
                }
            }
            foreach (var item in routevisits)
            {
                if (item.name == null)
                {
                    var index = routevisits.FindIndex(x => x.name == null);
                    routevisits[index].name = routevisits[index].name = "Desconocido";
                }
            }
            return Ok(routevisits);
        }

        public List<Routes> GetRoutesByUser(String userId)
        {
            if (userId == null)
            {
                return null;
            }
            List<Routes> routes = (from rr in _context.RelRouteUser
                                   join r in _context.Routes.Include("RouteVisit") on rr.IdRoute equals r.Id
                                   where rr.IdUser == userId
                                   select r).ToList();
            return routes;
        }

        public int GetTotalVisitsUserCount(List<Routes> routes)
        {
            int visitscount = 0;
            foreach (var item in routes)
            {
                if (item.RouteVisit.Count() > 0)
                {
                    visitscount = visitscount + item.RouteVisit.Count();
                }
            }
            return visitscount;
        }

        [HttpPost("GetStatiticsClicks")]
        [MapToApiVersion("2.0")]
        public IActionResult GetStatiticsClicks([FromBody]RelRouteUser data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var routes = GetRoutesByUser(data.IdUser);
            if (routes != null)
            {
                var haveRoute = routes.Where(x => x.Id == data.IdRoute).FirstOrDefault();
                if (haveRoute != null)
                {
                    var totalVisits = GetTotalVisitsUserCount(routes);
                    var selectedVisits = haveRoute.RouteVisit.Count();
                    totalVisits = (totalVisits > 0) ? totalVisits - selectedVisits : totalVisits;
                    List<Percentage> routeclicks = new List<Percentage>();
                    Percentage addToList = new Percentage();
                    addToList.name = "Otras";
                    addToList.y = totalVisits;
                    routeclicks.Add(addToList);
                    addToList = new Percentage();
                    addToList.name = "Seleccionada";
                    addToList.y = selectedVisits;
                    routeclicks.Add(addToList);
                    return Ok(routeclicks);
                }
                return NotFound("El usuario no tiene acceso de administrador a ésta ruta.");
            }
            return NotFound();
        }
        [HttpPut("UpdateFavorites")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> UpdateFavorites([FromBody] RelRouteUser data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var getFavStatus = _context.Favorites.Where(x => x.IdUser == data.IdUser && x.IdRoute == data.IdRoute).FirstOrDefault();
            if (getFavStatus != null)
            {
                getFavStatus.isFavorite = (getFavStatus.isFavorite) ? false : true;
                _context.Favorites.Update(getFavStatus);
            }
            else
            {
                getFavStatus = new Favorites();
                getFavStatus.isFavorite = true;
                getFavStatus.IdUser = data.IdUser;
                getFavStatus.IdRoute = data.IdRoute;
                _context.Favorites.Add(getFavStatus);
            }
            await _context.SaveChangesAsync();
            return Ok(getFavStatus.isFavorite);
        }
    }
}