﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Vokabular.Shared.AspNetCore.WebApiUtils.Attributes
{
    public class CustomRequireHttpsAttribute : RequireHttpsAttribute
    {
        protected override void HandleNonHttpsRequest(AuthorizationFilterContext filterContext)
        {
            var result = new ObjectResult("HTTPS required")
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            filterContext.Result = result;
        }
    }
}