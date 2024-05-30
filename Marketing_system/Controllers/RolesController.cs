using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;
using Marketing_system.BL.Service;
using Marketing_system.DA.Contracts.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Marketing_system.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/roles")]
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService= roleService;
        }
        [HttpGet("getAll")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
        {
            Log.Information("Requested all roles.");
            var roles = await _roleService.GetAllRoles();
            Log.Information("All roles returned!");
            return Ok(roles);
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleDto roleDto)
        {
            Log.Information("Requested role update.");
            var isUpdated = await _roleService.UpdateRole(roleDto);
            Log.Information("Role updated!");
            return Ok(isUpdated);
        }
    }
}
