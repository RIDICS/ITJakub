using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using Castle.DynamicProxy;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Core.Authentication
{
    public class AuthInterceptor : IInterceptor
    {
        private readonly AuthenticationManager m_authManager;
        private readonly Dictionary<RuntimeMethodHandle, bool> m_cachedMethods = new Dictionary<RuntimeMethodHandle, bool>();

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
                m_authManager.AuthenticateByCommunicationToken(WebOperationContext.Current.IncomingRequest.Headers["Communication_Token"]);
            }
            invocation.Proceed();
        }

        private Boolean CanIntercept(IInvocation invocation)
        {
            if (!m_cachedMethods.ContainsKey(invocation.Method.MethodHandle))
            {
                var attribute = invocation.Method.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof (AuthorizedMethodAttribute));
                if (attribute != null) 
                    m_cachedMethods[invocation.Method.MethodHandle] = true;
                else m_cachedMethods[invocation.Method.MethodHandle] = false;
            }
            return m_cachedMethods[invocation.Method.MethodHandle];
        }
    }
}