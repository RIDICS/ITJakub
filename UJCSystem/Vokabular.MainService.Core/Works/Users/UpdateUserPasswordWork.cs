using System.Net;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Jewelry;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;

namespace Vokabular.MainService.Core.Works.Users
{
    public class UpdateUserPasswordWork : UnitOfWorkBase
    {
        private readonly UserRepository m_userRepository;
        private readonly string m_authenticationToken;
        private readonly UpdateUserPasswordContract m_data;

        public UpdateUserPasswordWork(UserRepository userRepository, string authenticationToken, UpdateUserPasswordContract data) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_authenticationToken = authenticationToken;
            m_data = data;
        }

        protected override void ExecuteWorkImplementation()
        {
            var user = m_userRepository.GetUserByToken(m_authenticationToken);

            if (user == null || !CustomPasswordHasher.ValidatePassword(m_data.OldPassword, user.PasswordHash))
            {
                throw new HttpErrorCodeException("Invalid token or password", HttpStatusCode.Unauthorized);
            }

            var newPasswordHash = CustomPasswordHasher.CreateHash(m_data.NewPassword);
            user.PasswordHash = newPasswordHash;

            m_userRepository.Update(user);
        }
    }
}