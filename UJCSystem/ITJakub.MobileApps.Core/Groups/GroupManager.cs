using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using AutoMapper;
using ITJakub.MobileApps.Core.Applications;
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
        private readonly ApplicationManager m_applicationManager;
        private readonly UsersRepository m_usersRepository;


        public GroupManager(UsersRepository usersRepository,
            EnterCodeGenerator enterCodeGenerator,
            ApplicationManager applicationManager,
            int maxAttemptsToSave)
        {
            MaxAttemptsToSave = maxAttemptsToSave;
            m_usersRepository = usersRepository;
            m_enterCodeGenerator = enterCodeGenerator;
            m_applicationManager = applicationManager;
        }

        private int MaxAttemptsToSave { get; set; }

        public UserGroupsContract GetGroupByUser(long userId)
        {
            User user = m_usersRepository.GetUserWithGroups(userId);
            if (user == null)
                throw new FaultException("User not found.");

            var result = new UserGroupsContract
            {
                MemberOfGroup = Mapper.Map<IEnumerable<Group>, List<GroupInfoContract>>(user.MemberOfGroups),
                OwnedGroups = Mapper.Map<IEnumerable<Group>, List<OwnedGroupInfoContract>>(user.CreatedGroups)
            };

            return result;
        }

        public List<GroupInfoContract> GetMembershipGroups(long userId)
        {
            User user = m_usersRepository.GetUserWithGroups(userId); //TODO more optimized query ?
            if (user == null)
                throw new FaultException("User not found.");

            return Mapper.Map<IEnumerable<Group>, List<GroupInfoContract>>(user.MemberOfGroups);
        }


        public List<OwnedGroupInfoContract> GetOwnedGroups(long userId)
        {
            User user = m_usersRepository.GetUserWithGroups(userId); //TODO more optimized query ?
            if (user == null)
                throw new FaultException("User not found.");

            return Mapper.Map<IEnumerable<Group>, List<OwnedGroupInfoContract>>(user.CreatedGroups);
        }


        public CreateGroupResponse CreateGroup(long userId, string groupName)
        {
            User user = m_usersRepository.Load<User>(userId);

            var group = new Group {Author = user, CreateTime = DateTime.UtcNow, Name = groupName, State = GroupState.Created};

            int attempt = 0;
            while (attempt < MaxAttemptsToSave)
            {
                try
                {
                    group.EnterCode = m_enterCodeGenerator.GenerateCode();
                    m_usersRepository.Create(group);
                    return new CreateGroupResponse
                    {
                        EnterCode = group.EnterCode,
                        GroupId = group.Id
                    };
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

        public void AddUserToGroup(string groupAccessCode, long userId)
        {
            Group group = m_usersRepository.FindByEnterCode(groupAccessCode);
            if (group == null)
            {
                throw new FaultException("No group with this access code found.");
            }
            if (group.State == GroupState.Created || group.State == GroupState.Closed)
            {
                throw new FaultException("Group doesn't accept new members.");
            }

            var user = m_usersRepository.Load<User>(userId);
            group.Members.Add(user);
            m_usersRepository.Update(group);
        }

        public GroupDetailContract GetGroupDetails(long groupId)
        {
            var group = m_usersRepository.GetGroupDetails(groupId);
            if (group == null)
                throw new FaultException("Group not found.");

            var groupContract = Mapper.Map<Group, GroupDetailContract>(group);
            return groupContract;
        }

        public IList<GroupDetailsUpdateContract> GetGroupsUpdate(IList<OldGroupDetailsContract> oldGroups)
        {
            var groupsInfo = m_usersRepository.GetGroupMembers(oldGroups.Select(contract => contract.Id));
            var groupList = Mapper.Map<IList<Group>, IList<GroupDetailsUpdateContract>>(groupsInfo);

            foreach (var groupInfo in groupList)
            {
                var groupId = groupInfo.Id;
                var oldGroupInfo = oldGroups.First(group => group.Id == groupId);
                groupInfo.Members =
                    groupInfo.Members.Where(member => !oldGroupInfo.MemberIds.Contains(member.Id))
                        .ToList();
            }

            return groupList.Where(group => group.Members.Count > 0).ToList();
        }

        public void AssignTaskToGroup(long groupId, long taskId)
        {
            var group = m_usersRepository.FindById<Group>(groupId);
            if (group.State >= GroupState.Running)
                throw new FaultException("Can not change task. Group has been already started.");

            var task = m_usersRepository.Load<Task>(taskId);
            group.Task = task;

            m_usersRepository.Update(group);
        }

        public void UpdateGroupState(long groupId, GroupStateContract state)
        {
            var group = m_usersRepository.FindById<Group>(groupId);
            var newState = Mapper.Map<GroupStateContract, GroupState>(state);

            if (group.Task == null && newState > GroupState.AcceptMembers)
            {
                throw new FaultException("Can not change group state, because no task is assigned to the group.");
            }

            if (newState > group.State || (newState == GroupState.Running && group.State == GroupState.Paused))
            {
                group.State = newState;
                m_usersRepository.Update(group);
            }
            else
            {
                throw new FaultException("Can not change group state in this order.");
            }
        }

        public void RemoveGroup(long groupId)
        {
            var group = m_usersRepository.FindById<Group>(groupId);
            if (group.State != GroupState.Closed && group.State != GroupState.Created)
                throw new FaultException("Can not remove group until it is closed.");

            var rowKeys = m_usersRepository.GetRowKeysAndRemoveGroup(groupId);
            m_applicationManager.DeleteSynchronizedObjects(groupId, rowKeys);
        }

        public GroupStateContract GetGroupState(long groupId)
        {
            var group = m_usersRepository.FindById<Group>(groupId);
            if (group == null || group.State == GroupState.Created)
                throw new FaultException("Group not found.");

            return Mapper.Map<GroupState, GroupStateContract>(group.State);
        }

        public CreateGroupResponse DuplicateGroup(long userId, long groupId, string newGroupname)
        {
            var oldGroup = m_usersRepository.GetGroupWithMembers(groupId);
            if (oldGroup.State == GroupState.Created)
                throw new FaultException("Can not duplicate group in create state");

            var user = m_usersRepository.FindById<User>(userId);
            if (!user.Equals(oldGroup.Author))
            {
                throw new FaultException("You can duplicate only your groups");
            }

            var newGroup = new Group {Author = oldGroup.Author, CreateTime = DateTime.UtcNow, Name = newGroupname, State = GroupState.Created, Members = new List<User>()};

            foreach (var member in oldGroup.Members)
            {
                newGroup.Members.Add(member);
            }


            int attempt = 0;
            while (attempt < MaxAttemptsToSave)
            {
                try
                {
                    newGroup.EnterCode = m_enterCodeGenerator.GenerateCode();
                    m_usersRepository.Create(newGroup);
                    return new CreateGroupResponse
                    {
                        EnterCode = newGroup.EnterCode,
                        GroupId = newGroup.Id
                    };
                }
                catch (CreateEntityFailedException ex)
                {
                    ++attempt;

                    if (m_log.IsWarnEnabled)
                        m_log.WarnFormat("Could not save Group to Database. AttemptCount: '{0}', EntryCode: '{1}'. Exception: '{2}'", attempt,
                            newGroup.EnterCode, ex);
                }
            }

            if (m_log.IsErrorEnabled)
                m_log.ErrorFormat("Cannot create group, Maximum attemts to save exceeded");

            return null;
        }

        public string RenewGroupCode(long userId, long groupId)
        {
            var group = m_usersRepository.FindById<Group>(groupId);
            var user = m_usersRepository.FindById<User>(userId);

            if (!user.Equals(group.Author))
            {
                throw new FaultException("You can change group input code only your groups");
            }

            int attempt = 0;
            while (attempt < MaxAttemptsToSave)
            {
                try
                {
                    group.EnterCode = m_enterCodeGenerator.GenerateCode();
                    m_usersRepository.Save(group);
                    return group.EnterCode;
                }
                catch (CreateEntityFailedException ex)
                {
                    ++attempt;

                    if (m_log.IsWarnEnabled)
                        m_log.WarnFormat("Could not save Group to Database. AttemptCount: '{0}', EntryCode: '{1}'. Exception: '{2}'", attempt,
                            group.EnterCode, ex);
                }
            }

            throw new FaultException("Failed to renew code for group");
        }
    }
}