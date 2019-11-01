using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Users
{
    public class RegenerateSingleUserGroupNameWork : UnitOfWorkBase<string>
    {
        private readonly UserRepository m_userRepository;
        private readonly int m_groupId;
        private readonly CodeGenerator m_codeGenerator;

        public RegenerateSingleUserGroupNameWork(UserRepository userRepository, int groupId, CodeGenerator codeGenerator) : base(userRepository)
        {
            m_userRepository = userRepository;
            m_groupId = groupId;
            m_codeGenerator = codeGenerator;
        }

        protected override string ExecuteWorkImplementation()
        {
            var singleUserGroupSubwork = new SingleUserGroupSubwork(m_userRepository, m_codeGenerator);

            var userGroup = m_userRepository.FindById<SingleUserGroup>(m_groupId);
            userGroup.Name = singleUserGroupSubwork.GetUniqueName();

            m_userRepository.Update(userGroup);

            return userGroup.Name;
        }
    }
}