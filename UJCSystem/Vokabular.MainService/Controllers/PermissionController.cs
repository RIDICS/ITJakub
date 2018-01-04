using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts.Permission;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class PermissionController : BaseController
    {
        [HttpGet("special")]
        public List<SpecialPermissionContract> GetSpecialPermissions()
        {
            throw new NotImplementedException();
        }
    }
}