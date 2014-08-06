using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.ServiceModel.Web;
using Castle.Core.Internal;
using Castle.DynamicProxy;
using ITJakub.MobileApps.DataContracts;

namespace ITJakub.MobileApps.Core.Authentication
{
    public class AuthInterceptor : IInterceptor
    {
        private readonly AuthenticationManager m_authManager;
        private readonly Dictionary<RuntimeMethodHandle, AuthorizedMethodInfo> m_cachedMethods = new Dictionary<RuntimeMethodHandle, AuthorizedMethodInfo>();

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

                m_authManager.AuthenticateByCommunicationToken(WebOperationContext.Current.IncomingRequest.Headers["Communication_Token"], GetMinRoleAllowed(invocation),
                    GetUserIdentificator(invocation));
            }
            invocation.Proceed();
        }

        private long? GetUserIdentificator(IInvocation invocation)
        {
            var authMethodInfo = m_cachedMethods[invocation.Method.MethodHandle];
            if (authMethodInfo.AuthAttribute.UserIdParameterName.IsNullOrEmpty())
                return null;
            if (authMethodInfo.UserIdentificatorIndex == null)
                throw new ArgumentException("Specified user identificator name does not exist in method arguments.");
            long userIdentificator;
            if (!long.TryParse(invocation.Arguments[authMethodInfo.UserIdentificatorIndex.Value] as string, out userIdentificator))
                throw new WebFaultException(HttpStatusCode.BadRequest){Source = "User identificator was not correct data type"};
            return userIdentificator;
        }

        private UserRole GetMinRoleAllowed(IInvocation invocation)
        {
            return m_cachedMethods[invocation.Method.MethodHandle].AuthAttribute.MinRoleAllowed;
        }

        private Boolean CanIntercept(IInvocation invocation)
        {
            if (!m_cachedMethods.ContainsKey(invocation.Method.MethodHandle))
            {
                var authAttribute = invocation.Method.GetCustomAttribute<AuthorizedMethodAttribute>();
                if (authAttribute != null)
                    m_cachedMethods[invocation.Method.MethodHandle] = new AuthorizedMethodInfo
                    {
                        AuthAttribute = authAttribute,
                        UserIdentificatorIndex = GetIdentificatorIndexByName(invocation.Method, authAttribute.UserIdParameterName)
                    };
                else m_cachedMethods[invocation.Method.MethodHandle] = null;
            }
            return m_cachedMethods[invocation.Method.MethodHandle] != null;
        }


        private int? GetIdentificatorIndexByName(MethodInfo method, string paramName)
        {
            var parameters = method.GetParameters();
            for (var nameIndex = 0; nameIndex < parameters.Length; nameIndex++)
            {
                if (parameters[nameIndex].Name.Equals(paramName))
                {
                    return nameIndex;
                }
            }
            return null;
        }
    }


    internal class AuthorizedMethodInfo
    {
        public AuthorizedMethodAttribute AuthAttribute { get; set; }
        public int? UserIdentificatorIndex { get; set; }
    }
}