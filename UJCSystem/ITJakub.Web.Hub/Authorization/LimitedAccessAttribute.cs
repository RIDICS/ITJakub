using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Vokabular.Shared;
using Vokabular.Shared.Options;

namespace ITJakub.Web.Hub.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LimitedAccessAttribute : TypeFilterAttribute
    {
        public LimitedAccessAttribute(PortalType requiredPortalType) : base(typeof(LimitedAccessAttributeImpl))
        {
            Arguments = new object[]
            {
                requiredPortalType
            };
        }

        private class LimitedAccessAttributeImpl : IAuthorizationFilter
        {
            private readonly PortalConfigOption m_portalConfigOption;
            private readonly PortalType m_requiredPortalType;

            public LimitedAccessAttributeImpl(PortalType requiredPortalType, IOptions<PortalConfigOption> portalConfigOption)
            {
                m_requiredPortalType = requiredPortalType;
                m_portalConfigOption = portalConfigOption.Value;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                if (m_requiredPortalType != m_portalConfigOption.PortalType)
                {
                    context.Result = new UnauthorizedResult();
                }
            }
        }
    }
}
