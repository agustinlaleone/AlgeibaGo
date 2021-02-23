using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AlgeibaGoAPI.Models;
using AlgeibaGoAPI.Data;
using System.Linq;
using System.Collections.Generic;
using AlgeibaGoAPI.Controllers;
using AlgeibaGoAPI.Extensions;

namespace AlgeibaGoAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin ,User")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AlgeibaGoContext _contextAlg;
        private readonly ApplicationDbContext _context;
        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, AlgeibaGoContext contextAlg)
        {
            _userManager = userManager;
            _context = context;
            _contextAlg = contextAlg;

        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _context.Users.ToList();
                if (users != null)
                {
                    return Ok(users);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("GetCount")]
        public long GetCount(PagingParameters param)
        {
            return (_context.Users == null || _context.Users.Count() == 0)? 0 : _context.Users.Count();
        }

        [HttpGet("GetAllUsers")]
        [MapToApiVersion("2.0")]
        public IList<User> GetAllUsers()
        {
            if (_contextAlg.AspNetUsers == null || _contextAlg.AspNetUsers.Count() == 0) return null;

            var aspUsers = (from asp in _contextAlg.AspNetUsers select new User { Id = asp.Id, UserName = asp.UserName }).ToList();

            var usersQuery = aspUsers.AsQueryable();

            return aspUsers;
        }


        [HttpPost("GetPaged")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public IEnumerable<ApplicationUser> GetUsersPaged(PagingParameters param)
        {
            if (_context.Users == null || _context.Users.Count() == 0) return null;

            List<ApplicationUser> users = _context.Users.OrderBy(x => x.UserName).ToList();

            IQueryable<ApplicationUser> usersQuery = users.AsQueryable().Where(u => u.UserName.StartsWith(param.filter.ToString())).Skip(param.pageIndex * param.pageSize).Take(param.pageSize);

            return usersQuery;
        }

        [HttpDelete]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if(user != null)
                {
                    List<RelRouteUser> relRouteUsers = _contextAlg.RelRouteUser.Where(x => x.IdUser == userId).ToList();
                    if(relRouteUsers.Count != 0) return BadRequest("El usuario está relacionado a Rutas, no se puede eliminar");
                    var roles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, roles);
                    await _userManager.DeleteAsync(user);
                }
                return Ok();
            } catch (Exception)
            {
                return StatusCode(500, "Surgió un problema al realizar la operación");
            }
            
        }

        [HttpPost]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> AddUser(User user)
        {
            try
            {
                var userToAdd = await _userManager.FindByNameAsync(user.UserName);
                if(userToAdd != null)
                {
                    return BadRequest();
                }
                userToAdd = new ApplicationUser { Email = user.Email, UserName = user.UserName, PersonName = user.PersonName, PersonSurname = user.PersonSurname };
                await _userManager.CreateAsync(userToAdd, user.Password);
                foreach (var role in user.Roles)
                {
                    await _userManager.AddToRoleAsync(userToAdd, role);
                }
                return Ok();
            } catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> UpdateUser(User user)
        {
            try
            {
                var userToUpdate = await _userManager.FindByNameAsync(user.UserName);
                userToUpdate.Email = user.Email;
                userToUpdate.UserName = user.UserName;
                userToUpdate.PersonName = user.PersonName;
                userToUpdate.PersonSurname = user.PersonSurname;
                await _userManager.UpdateAsync(userToUpdate);
                await _userManager.UpdateNormalizedEmailAsync(userToUpdate);
                await _userManager.UpdateNormalizedUserNameAsync(userToUpdate);
                var roles = await _userManager.GetRolesAsync(userToUpdate);
                try
                {
                    await _userManager.RemoveFromRolesAsync(userToUpdate, roles);
                    foreach(var role in user.Roles)
                    {
                        await _userManager.AddToRoleAsync(userToUpdate, role);
                    }
                    return Ok();
                }
                catch (Exception)
                {
                    return StatusCode(400, "Hubo un problema al modificar los roles");
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpPost("ChangePassword")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> ChangePasswordAsync(User userFront)
        {
            try
            {
                if (_context.Users == null || _context.Users.Count() == 0) return null;

                if (userFront.Password == null) return StatusCode(400, "No fue posible cambiar la contraseña.");
                var user = await _userManager.FindByIdAsync(userFront.Id);
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, userFront.Password);
            
                return Ok();
            } catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}