using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AlgeibaGoAPI.Models;
using System.Linq;
using System.Collections.Generic;

namespace AlgeibaGoAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin ,User")]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AlgeibaGoContext _context;
        public RolesController(UserManager<ApplicationUser> userManager, AlgeibaGoContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> GetRolesByUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    return Ok(roles);
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
        
        [HttpGet("GetAll")]
        [MapToApiVersion("2.0")]
        public IActionResult GetAllRoles()
        {
            try
            {
                var roles = _context.AspNetRoles.ToList();
                List<string> roleList = new List<string>();
                foreach (var role in roles)
                {
                    roleList.Add(role.Name);
                }
                return Ok(roleList);
                
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        
        [HttpPost]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> AddRole(string userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await _userManager.AddToRoleAsync(user, role);
                    return Ok();
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

        [HttpDelete]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> DeleteRole(string userId, string roleToDelete)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Count == 1)
                    {
                        return BadRequest();
                    }
                    var roleresult = await _userManager.RemoveFromRoleAsync(user, roleToDelete);

                    return Ok(roleresult);
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

        [HttpPost("SaveRoles")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult> SaveRoles(User userData)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userData.Id);
            var roles = await _userManager.GetRolesAsync(user);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                foreach (var role in roles)
                {
                    await DeleteRole(user.Id, role);
                }
            }

            foreach (var role in userData.Roles)
            {
                await AddRole(user.Id, role);
            }
            return Ok();

        }

    }
}