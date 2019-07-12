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
            private readonly PortalOption m_portalOption;
            private readonly PortalType m_requiredPortalType;

            public LimitedAccessAttributeImpl(PortalType requiredPortalType, IOptions<PortalOption> portalConfigOption)
            {
                m_requiredPortalType = requiredPortalType;
                m_portalOption = portalConfigOption.Value;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                if (m_requiredPortalType != m_portalOption.PortalType)
                {
                    context.Result = new UnauthorizedResult();
                }
            }
        }
    }
}
