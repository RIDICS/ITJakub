﻿using System.Collections.Generic;
using System.Linq;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace ITJakub.MobileApps.DataEntities.Database.Repositories
{
    [Transactional]
    public class UsersRepository : NHibernateTransactionalDao
    {
        public UsersRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public override object Create(object instance)
        {
            try
            {
                return base.Create(instance);
            }
            catch (DataException ex)
            {
                throw new CreateEntityFailedException(ex.Message, ex.InnerException);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User FindByEmailAndProvider(string email, AuthenticationProviders authenticationProvider)
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<User>()
                    .Where(x => x.Email == email && x.AuthenticationProvider == authenticationProvider)
                    .SingleOrDefault();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User GetUserByCommunicationToken(string communicationToken)
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<User>()
                    .Where(x => x.CommunicationToken == communicationToken)
                    .SingleOrDefault();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User GetUserWithGroups(long userId)
        {
            using (ISession session = GetSession())
            {
                session.QueryOver<User>()
                    .Where(x => x.Id == userId)
                    .Fetch(x => x.CreatedGroups).Eager
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .Future();

                session.QueryOver<Group>()
                    .Where(group => group.Author.Id == userId)
                    .JoinQueryOver<User>(group => group.Members, JoinType.LeftOuterJoin)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .List();
                
                var u1 = session.QueryOver<User>()
                    .Where(x => x.Id == userId)
                    .JoinQueryOver<Group>(x => x.MemberOfGroups, JoinType.LeftOuterJoin)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .Future();

                var groupIds = QueryOver.Of<Group>()
                    .Right.JoinQueryOver<User>(x => x.Members)
                    .Where(user => user.Id == userId)
                    .Select(x => x.Id);

                var groupWithMembers = session.QueryOver<Group>()
                    .WithSubquery
                    .WhereProperty(x => x.Id).In(groupIds)
                    .JoinQueryOver<User>(x => x.Members, JoinType.LeftOuterJoin)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .Future();

                var result = u1.Single();
                result.MemberOfGroups = groupWithMembers.ToList();
                return result;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Group> GetGroupMembers(IEnumerable<long> groupIds)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<Group>()
                    .WhereRestrictionOn(group => group.Id).IsIn(groupIds.ToList())
                    .JoinQueryOver<User>(group => group.Members, JoinType.LeftOuterJoin)
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .List();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Group FindByEnterCode(string enterCode)
        {
            using (ISession session = GetSession())
            {
                return session.QueryOver<Group>()
                    .Where(x => x.EnterCode == enterCode)
                    .Fetch(x => x.Members).Eager
                    .SingleOrDefault();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Group GetGroupDetails(long groupId)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<Group>()
                    .Where(x => x.Id == groupId)
                    .Fetch(x => x.Members).Eager
                    .JoinQueryOver(x => x.Task, JoinType.LeftOuterJoin)
                    .JoinQueryOver(x => x.Author, JoinType.LeftOuterJoin)
                    .SingleOrDefault();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Task> GetTasksByApplication(int applicationId)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<Task>()
                    .Where(x => x.Application.Id == applicationId)
                    .Fetch(x => x.Author).Eager
                    .OrderBy(x => x.CreateTime).Desc
                    .List();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<Task> GetTasksByAuthor(long authorId)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<Task>()
                    .Where(x => x.Author.Id == authorId)
                    .Fetch(x => x.Author).Eager
                    .List();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Group GetGroupWithTask(long groupId)
        {
            using (var session = GetSession())
            {
                var group = session.QueryOver<Group>()
                    .Where(x => x.Id == groupId)
                    .Fetch(x => x.Task).Eager
                    .SingleOrDefault();

                return group;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IEnumerable<string> GetRowKeysAndRemoveGroup(long groupId)
        {
            using (var session = GetSession())
            {
                var group = session.QueryOver<Group>().Where(x => x.Id == groupId)
                    .Fetch(x => x.SynchronizedObjects).Eager
                    .SingleOrDefault<Group>();
                
                session.Delete(group);

                var rowKeys = group.SynchronizedObjects.OfType<SynchronizedObject>().Select(o => o.ObjectExternalId);

                return rowKeys;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Institution FindInstitutionByEnterCode(string enterCode)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<Institution>()
                    .Where(x => x.EnterCode == enterCode)
                    .SingleOrDefault();
            }
        }
                
        [Transaction(TransactionMode.Requires)]
        public virtual Group GetGroupWithMembers(long groupId)
        {
            using (var session = GetSession())
            {
                var group = session.QueryOver<Group>().Where(x => x.Id == groupId)
                     .Fetch(x => x.Members).Eager
                    .SingleOrDefault<Group>();

                return group;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual User GetUserWithInstititutionById(long userId)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<User>().Where(x => x.Id == userId)
                    .Fetch(x => x.Institution).Eager.SingleOrDefault<User>();
            }
        }
    }
}