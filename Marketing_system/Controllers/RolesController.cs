using Marketing_system.BL.Contracts.DTO;
using Marketing_system.BL.Contracts.IService;
using Marketing_system.BL.Service;
using Marketing_system.DA.Contracts.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var roles = await _roleService.GetAllRoles();
            return Ok(roles);
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleDto roleDto)
        {
            var isUpdated = await _roleService.UpdateRole(roleDto);
            return Ok(isUpdated);
        }
    }
}
