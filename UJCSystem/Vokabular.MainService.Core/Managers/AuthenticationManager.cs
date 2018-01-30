using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.Core.Errors;
using Vokabular.MainService.Core.Managers.Authentication;
using Vokabular.MainService.Core.Works.Users;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.Const;

namespace Vokabular.MainService.Core.Managers
{
    public class AuthenticationManager
    {
        private readonly UserRepository m_userRepository;
        private readonly PermissionRepository m_permissionRepository;
        private readonly ICommunicationTokenGenerator m_communicationTokenGenerator;
        private readonly DefaultUserProvider m_defaultUserProvider;
        private readonly ICommunicationTokenProvider m_communicationTokenProvider;

        public AuthenticationManager(UserRepository userRepository, PermissionRepository permissionRepository,
            ICommunicationTokenGenerator communicationTokenGenerator, DefaultUserProvider defaultUserProvider,
            ICommunicationTokenProvider communicationTokenProvider)
        {
            m_userRepository = userRepository;
            m_permissionRepository = permissionRepository;
            m_communicationTokenGenerator = communicationTokenGenerator;
            m_defaultUserProvider = defaultUserProvider;
            m_communicationTokenProvider = communicationTokenProvider;
        }

        public User GetCurrentUser(bool returnDefaultIfNull)
        {
            var communicationToken = m_communicationTokenProvider.GetCommunicationToken();
            if (communicationToken == null)
            {
                if (returnDefaultIfNull)
                {
                    return m_defaultUserProvider.GetDefaultUser();
                }

                throw new AuthenticationException("User not signed in");
            }

            var user = m_userRepository.InvokeUnitOfWork(x => x.GetUserByToken(communicationToken));

            if (user == null || !m_communicationTokenGenerator.ValidateTokenFormat(communicationToken))
            {
                throw new AuthenticationException("Invalid communication token");
            }

            return user;
        }

        public int GetCurrentUserId()
        {
            return GetCurrentUser(false).Id;
        }
        
        public SignInResultContract RenewCommunicationToken()
        {
            var userId = GetCurrentUserId();
            var work = new RenewCommunicationTokenWork(m_userRepository, m_communicationTokenGenerator, userId);
            work.Execute();

            return new SignInResultContract
            {
                CommunicationToken = work.CommunicationToken
            };
        }

        public SignInResultContract SignIn(SignInContract data)
        {
            var work = new SignInWork(m_userRepository, m_permissionRepository, m_communicationTokenGenerator, data);
            work.Execute();

            var roles = ConvertActionSpecialPermissionsToRoles(work.ActionSpecialPermissions.ToList());

            return new SignInResultContract
            {
                CommunicationToken = work.CommunicationToken,
                Roles = roles,
            };
        }

        public void SignOut()
        {
            var userId = GetCurrentUserId();
            new SignOutWork(m_userRepository, userId).Execute();
        }

        private List<string> ConvertActionSpecialPermissionsToRoles(List<SpecialPermission> actionSpecialPermissions)
        {
            var roles = new List<string>();

            if (actionSpecialPermissions.Count != 0)
            {
                roles.Add(CustomRole.CanViewAdminModule);
            }

            if (actionSpecialPermissions.OfType<UploadBookPermission>().Count() != 0)
            {
                roles.Add(CustomRole.CanUploadBooks);
            }

            if (actionSpecialPermissions.OfType<NewsPermission>().Count() != 0)
            {
                roles.Add(CustomRole.CanAddNews);
            }

            if (actionSpecialPermissions.OfType<ManagePermissionsPermission>().Count() != 0)
            {
                roles.Add(CustomRole.CanManagePermissions);
            }

            if (actionSpecialPermissions.OfType<FeedbackPermission>().Count() != 0)
            {
                roles.Add(CustomRole.CanManageFeedbacks);
            }

            if (actionSpecialPermissions.OfType<ReadLemmatizationPermission>().Count() != 0)
            {
                roles.Add(CustomRole.CanReadLemmatization);
            }

            if (actionSpecialPermissions.OfType<EditLemmatizationPermission>().Count() != 0)
            {
                roles.Add(CustomRole.CanEditLemmatization);
            }

            if (actionSpecialPermissions.OfType<DerivateLemmatizationPermission>().Count() != 0)
            {
                roles.Add(CustomRole.CanDerivateLemmatization);
            }

            if (actionSpecialPermissions.OfType<EditionPrintTextPermission>().Count() != 0)
            {
                roles.Add(CustomRole.CanEditionPrint);
            }

            if (actionSpecialPermissions.OfType<EditStaticTextPermission>().Count() != 0)
            {
                roles.Add(CustomRole.CanEditStaticText);
            }

            return roles;
        }
    }
}
