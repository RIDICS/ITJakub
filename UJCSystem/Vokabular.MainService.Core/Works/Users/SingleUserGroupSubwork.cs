using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.Core.Utils;

namespace Vokabular.MainService.Core.Works.Users
{
    public class SingleUserGroupSubwork
    {
        private readonly UserRepository m_userRepository;
        private readonly CodeGenerator m_codeGenerator;

        public SingleUserGroupSubwork(UserRepository userRepository, CodeGenerator codeGenerator)
        {
            m_userRepository = userRepository;
            m_codeGenerator = codeGenerator;
        }

        public string GetUniqueName()
        {
            string uniqueName;
            IList<SingleUserGroup> singleUserGroups;

            do
            {
                uniqueName = m_codeGenerator.Generate(CodeGenerator.UserGroupNameLength);
                singleUserGroups = m_userRepository.FindSingleUserGroupsByName(uniqueName);
            } while (singleUserGroups.Count > 0);

            return uniqueName;
        }
    }
}