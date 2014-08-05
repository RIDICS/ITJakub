using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Web;
using Castle.DynamicProxy;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Core.Authentication
{
    public class AuthInterceptor : IInterceptor
    {
        private readonly AuthenticationManager m_authManager;
        private readonly Dictionary<RuntimeMethodHandle, AuthorizedMethodAttribute> m_cachedMethods = new Dictionary<RuntimeMethodHandle, AuthorizedMethodAttribute>();

        public AuthInterceptor(AuthenticationManager authManager)
        {
            m_authManager = authManager;
        }


        public void Intercept(IInvocation invocation)
        {
            if (CanIntercept(invocation))
            {
                if (WebOperationContext.Current == null)
                    throw new ArgumentException("Missing context");

                m_authManager.AuthenticateByCommunicationToken(WebOperationContext.Current.IncomingRequest.Headers["Communication_Token"], MinRoleAllowed(invocation));
            }
            invocation.Proceed();
        }

        private Role MinRoleAllowed(IInvocation invocation)
        {
            return m_cachedMethods[invocation.Method.MethodHandle].MinRoleAllowed;
        }

        private Boolean CanIntercept(IInvocation invocation)
        {
            if (!m_cachedMethods.ContainsKey(invocation.Method.MethodHandle))
            {
                AuthorizedMethodAttribute authAttribute = invocation.Method.GetCustomAttribute<AuthorizedMethodAttribute>();
                if (authAttribute != null)
                    m_cachedMethods[invocation.Method.MethodHandle] = authAttribute;
                else m_cachedMethods[invocation.Method.MethodHandle] = null;
            }
            return m_cachedMethods[invocation.Method.MethodHandle] != null;
        }
    }
}