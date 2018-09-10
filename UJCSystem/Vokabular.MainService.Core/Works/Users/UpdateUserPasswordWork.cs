using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Jewelry;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class UpdateUserPasswordWork : UnitOfWorkBase
    {
        private readonly UserRepository m_userRepository;
        private readonly int m_userId;
        private readonly UpdateUserPasswordContract m_data;

        public UpdateUserPasswordWork(UserRepository userRepository, int userId, UpdateUserPasswordContract data) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_userId = userId;
            m_data = data;
        }

        protected override void ExecuteWorkImplementation()
        {
            var user = m_userRepository.FindById<User>(m_userId);

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