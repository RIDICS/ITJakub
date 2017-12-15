using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.Users
{
    public class UpdateUserWork : UnitOfWorkBase
    {
        private readonly UserRepository m_userRepository;
        private readonly string m_authenticationToken;
        private readonly UpdateUserContract m_data;

        public UpdateUserWork(UserRepository userRepository, string authenticationToken, UpdateUserContract data) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_authenticationToken = authenticationToken;
            m_data = data;
        }

        protected override void ExecuteWorkImplementation()
        {
            var user = m_userRepository.GetUserByToken(m_authenticationToken);

            user.AvatarUrl = m_data.AvatarUrl;
            user.Email = m_data.Email;
            user.FirstName = m_data.FirstName;
            user.LastName = m_data.LastName;
            
            m_userRepository.Update(user);
        }
    }
}