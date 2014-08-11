using System.Collections.Generic;
using AutoMapper;
using ITJakub.MobileApps.DataContracts.Groups;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core.Groups
{
    public class GroupManager
    {
        private readonly UsersRepository m_usersRepository;

        public GroupManager(UsersRepository usersRepository)
        {
            m_usersRepository = usersRepository;
        }

        public UserGroupsContract GetGroupByUser(long userId)
        {
            var user = m_usersRepository.GetUserWithGroups(userId);

            var result = new UserGroupsContract
            {
                MemberOfGroup = Mapper.Map<IEnumerable<Group>, List<GroupDetailContract>>(user.MemberOfGroups),
                OwnedGroups = Mapper.Map<IEnumerable<Group>, List<OwnedDetailGroupContract>>(user.CreatedGroups)
            };

            return result;
        }
    }
}
