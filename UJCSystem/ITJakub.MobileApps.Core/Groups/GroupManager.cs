using System;
using System.Collections.Generic;
using System.Reflection;
using AutoMapper;
using ITJakub.MobileApps.DataContracts.Groups;
using ITJakub.MobileApps.DataEntities;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using ITJakub.MobileApps.DataEntities.Database.Repositories;
using log4net;

namespace ITJakub.MobileApps.Core.Groups
{
    public class GroupManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly EnterCodeGenerator m_enterCodeGenerator;
        private readonly UsersRepository m_usersRepository;


        public GroupManager(UsersRepository usersRepository, EnterCodeGenerator enterCodeGenerator, int maxAttemptsToSave)
        {
            MaxAttemptsToSave = maxAttemptsToSave;
            m_usersRepository = usersRepository;
            m_enterCodeGenerator = enterCodeGenerator;
        }

        private int MaxAttemptsToSave { get; set; }

        public UserGroupsContract GetGroupByUser(long userId)
        {
            User user = m_usersRepository.GetUserWithGroups(userId);

            var result = new UserGroupsContract
            {
                MemberOfGroup = Mapper.Map<IEnumerable<Group>, List<GroupDetailContract>>(user.MemberOfGroups),
                OwnedGroups = Mapper.Map<IEnumerable<Group>, List<OwnedDetailGroupContract>>(user.CreatedGroups)
            };

            return result;
        }

        public CreateGroupResponse CreateGroup(long userId, string groupName)
        {
            User user = m_usersRepository.Load(userId);
            
            var group = new Group {Author = user, CreateTime = DateTime.UtcNow, Name = groupName, IsActive = true};

            int attempt = 0;
            while (attempt < MaxAttemptsToSave)
            {
                try
                {
                    group.EnterCode = m_enterCodeGenerator.GenerateCode();
                    m_usersRepository.Create(group);
                    return new CreateGroupResponse {EnterCode = group.EnterCode};
                }
                catch (CreateEntityFailedException ex)
                {
                    ++attempt;

                    if (m_log.IsWarnEnabled)
                        m_log.WarnFormat("Could not save Group to Database. AttemptCount: '{0}', EntryCode: '{1}'. Exception: '{2}'", attempt,
                            group.EnterCode, ex);
                }
            }

            if (m_log.IsErrorEnabled)
                m_log.ErrorFormat("Cannot create group, Maximum attemts to save exceeded");

            return null;
        }
    }
}