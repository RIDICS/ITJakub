using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Transactions;
using Castle.Facilities.NHibernate;
using Castle.Transactions;
using log4net;
using NHibernate;
using NHibernate.Exceptions;

namespace ITJakub.DataEntities.Database.Daos
{
    public class NHibernateDao<T> where T : IEquatable<T>
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

        public virtual T FindById(object id)
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

        public virtual T Load(object id)
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

        /// <summary>
        /// returning primary key
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual object Create(T instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    return session.Save(instance);
                }
                catch (Exception ex)
                {
                    throw new DataException(
                        string.Format("Create operation failed for type:{0}", instance.GetType().Name), ex);
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
                    throw new DataException(
                        string.Format("Create operation failed for type:{0}", instance.GetType().Name), ex);
                }
            }
        }

        public virtual void Delete(T instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Delete(instance);
                }
                catch (Exception ex)
                {
                    throw new DataException(
                        string.Format("Delete operation failed for type:{0}", instance.GetType().Name), ex);
                }
            }
        }

        public virtual void DeleteAll()
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.Delete(String.Format("from {0}", typeof(T).Name));
                }
                catch (Exception ex)
                {
                    throw new DataException(string.Format("Delete operation failed for type:{0}", typeof(T).Name), ex);
                }
            }
        }

        public virtual void Update(T instance)
        {
            Update((object)instance);
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
                    throw new DataException(
                        string.Format("Update operation failed for type:{0}", instance.GetType().Name), ex);
                }
            }
        }

        public virtual void Save(object instance)
        {
            using (ISession session = GetSession())
            {
                try
                {
                    session.SaveOrUpdate(instance);
                }
                catch (Exception ex)
                {
                    throw new DataException(
                        string.Format("Save or Update operation failed for type:{0} ", instance.GetType().Name), ex);
                }
            }
        }

        public virtual void Save(T instance)
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
                throw new DataException(
                    string.Format("Save or Update operation failed for type:{0} ", instance.GetType().Name), ex);
            }
        }

        public virtual void SaveAll(IEnumerable<T> data)
        {
            using (ISession session = GetSession())
            {
                foreach (T o in data)
                {
                    Save(o, session);
                }
            }
        }

        public virtual void SaveAll(IEnumerable data)
        {
            using (ISession session = GetSession())
            {
                foreach (object o in data)
                {
                    Save(o, session);
                }
            }
        }
    }


    public class NHibernateTransactionalDao<T> : NHibernateDao<T> where T : IEquatable<T>
    {
        public NHibernateTransactionalDao(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionScopeOption.Required)]
        public override T FindById(object id)
        {
            return base.FindById(id);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override T Load(object id)
        {
            return base.Load(id);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override object Create(T instance)
        {
            return base.Create(instance);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override void Delete(T instance)
        {
            base.Delete(instance);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override void Update(T instance)
        {
            base.Update(instance);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override void DeleteAll()
        {
            base.DeleteAll();
        }

        [Transaction(TransactionScopeOption.Required)]
        public override void Save(object instance)
        {
            base.Save(instance);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override void Save(T instance)
        {
            base.Save(instance);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override void SaveAll(IEnumerable<T> data)
        {
            base.SaveAll(data);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override object Create(object instance)
        {
            return base.Create(instance);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override void SaveAll(IEnumerable data)
        {
            base.SaveAll(data);
        }

        [Transaction(TransactionScopeOption.Required)]
        public override void Update(object instance)
        {
            base.Update(instance);
        }
    }
}