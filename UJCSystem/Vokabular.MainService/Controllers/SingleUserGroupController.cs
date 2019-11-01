using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridics.Core.Structures.Shared;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class SingleUserGroupController : BaseController
    {
        private readonly RoleManager m_roleManager;

        public SingleUserGroupController(RoleManager roleManager)
        {
            m_roleManager = roleManager;
        }

        [Authorize(PermissionNames.ManageUserRoles)]
        [HttpGet("autocomplete")]
        public IList<UserGroupContract> GetAutocomplete([FromQuery] string query, [FromQuery] int? count, [FromQuery] bool includeSearchInUsers)
        {
            var result = m_roleManager.GetSingleUserGroupAutocomplete(query, count, includeSearchInUsers);
            return result;
        }

        // TODO check permissions
        [HttpPost("{groupId}/regenerate-name")]
        public IActionResult RegenerateSingleUserGroupName(int groupId)
        {
            var newCode = m_roleManager.RegenerateSingleUserGroupName(groupId);
            return Ok(newCode);
        }
    }
}