using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Transactions;
using Castle.Facilities.NHibernate;
using Castle.Transactions;
using log4net;
using NHibernate;

namespace ITJakub.DataEntities.Database.Daos
{
    public class NHibernateDao
    {
        protected static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISessionManager m_sessionManager;

        public NHibernateDao(ISessionManager sessionManager)
        {
            m_sessionManager = sessionManager;
        }

        protected ISessionManager SessionManager
        {
            get { return m_sessionManager; }
        }

        protected ISession GetSession()
        {

            var session = m_sessionManager.OpenSession();
            //if (m_log.IsDebugEnabled)
            //    m_log.DebugFormat("Getting session with flush mode: {0}", session.FlushMode);

            return session;
        }

        public virtual object FindById(Type type, object id)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    return session.Get(type, id);
                }
                catch (Exception ex)
                {
                    throw new DataException(string.Format("Get by id operation failed for type:{0}", type.Name), ex);
                }
            }
        }

        public virtual T FindById<T>(object id)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    return (T)session.Get(typeof(T), id);
                }
                catch (Exception ex)
                {
                    throw new DataException(string.Format("Get by id operation failed for type:{0}", typeof(T).Name), ex);
                }
            }
        }

        public virtual object Load(Type type, object id)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    return session.Load(type, id);
                }
                catch (ObjectNotFoundException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new DataException(string.Format("Load by id operation failed for type:{0}", type.Name), ex);
                }
            }
        }
        public virtual T Load<T>(object id)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    return (T)session.Load(typeof(T), id);
                }
                catch (ObjectNotFoundException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new DataException(string.Format("Load by id operation failed for type:{0}", typeof(T).Name), ex);
                }
            }
        }

        public virtual object Create(object instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    return session.Save(instance);
                }
                catch (Exception ex)
                {
                    throw new DataException(string.Format("Create operation failed for type:{0}", instance.GetType().Name), ex);
                }
            }
        }

        public virtual IList<object> CreateAll(IEnumerable data)
        {
            var result = new List<object>();
            using (ISession session = GetSession())
            {
                foreach (var instance in data)
                {
                    try
                    {
                        var id = session.Save(instance);
                        result.Add(id);
                    }
                    catch (Exception ex)
                    {
                        throw new DataException(string.Format("Create operation failed for type:{0}", instance.GetType().Name), ex);
                    }
                }
            }
            return result;
        }

        public virtual void Delete(object instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Delete(instance);
                }
                catch (Exception ex)
                {
                    throw new DataException(string.Format("Delete operation failed for type:{0}", instance.GetType().Name), ex);
                }
            }
        }

        public virtual void Update(object instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Update(instance);
                }
                catch (Exception ex)
                {
                    throw new DataException(string.Format("Update operation failed for type:{0}", instance.GetType().Name), ex);
                }
            }
        }

        public virtual void DeleteAll(Type type)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Delete(String.Format("from {0}", type.Name));
                }
                catch (Exception ex)
                {
                    throw new DataException(string.Format("Delete operation failed for type:{0}", type.Name), ex);
                }
            }
        }

        public virtual void Save(object instance)
        {
            using (ISession session = GetSession())
            {
                Save(instance, session);
            }
        }

        protected virtual void Save(object instance, ISession session)
        {
            try
            {
                session.SaveOrUpdate(instance);
            }
            catch (Exception ex)
            {
                throw new DataException(string.Format("Save or Update operation failed for type:{0} ", instance.GetType().Name), ex);
            }
        }

        public virtual void SaveAll(IEnumerable data)
        {
            using (ISession session = GetSession())
            {
                foreach (var o in data)
                {
                    Save(o, session);
                }
            }
        }
    }

    public class NHibernateTransactionalDao : NHibernateDao
    {
        public NHibernateTransactionalDao(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionScopeOption.Required)]
        public override object Create(object instance)
        {
            return base.Create(instance);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override void Update(object instance)
        {
            base.Update(instance);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override void Delete(object instance)
        {
            base.Delete(instance);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override void Save(object instance)
        {
            base.Save(instance);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override IList<object> CreateAll(IEnumerable data)
        {
            return base.CreateAll(data);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override void DeleteAll(Type instanceType)
        {
            base.DeleteAll(instanceType);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override void SaveAll(IEnumerable data)
        {
            base.SaveAll(data);
        }
    }
}