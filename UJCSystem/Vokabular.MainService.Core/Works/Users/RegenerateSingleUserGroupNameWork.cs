using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class RegenerateSingleUserGroupNameWork : UnitOfWorkBase<string>
    {
        private readonly UserRepository m_userRepository;
        private readonly int m_userId;
        private readonly CodeGenerator m_codeGenerator;

        public RegenerateSingleUserGroupNameWork(UserRepository userRepository, int userId, CodeGenerator codeGenerator) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_userId = userId;
            m_codeGenerator = codeGenerator;
        }

        protected override string ExecuteWorkImplementation()
        {
            var singleUserGroupSubwork = new SingleUserGroupSubwork(m_userRepository, m_codeGenerator);

            var userGroup = m_userRepository.GetSingleUserGroup(m_userId);
            userGroup.Name = singleUserGroupSubwork.GetUniqueName();

            m_userRepository.Update(userGroup);

            return userGroup.Name;
        }
    }
}