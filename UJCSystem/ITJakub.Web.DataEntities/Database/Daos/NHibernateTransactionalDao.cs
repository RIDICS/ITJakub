using System;
using System.Collections;
using System.Data;
using System.Reflection;
using Castle.Facilities.NHibernateIntegration;
using Castle.Facilities.NHibernateIntegration.Util;
using Castle.Services.Transaction;
using log4net;
using NHibernate;
using NHibernate.Collection;
using NHibernate.Proxy;

namespace ITJakub.Web.DataEntities.Database.Daos
{
    public class NHibernateDao
    {
        protected static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISessionManager m_sessionManager;
        private string m_sessionFactoryAlias;

        public NHibernateDao(ISessionManager sessionManager)
        {
            m_sessionManager = sessionManager;
        }

        protected ISessionManager SessionManager
        {
            get { return m_sessionManager; }
        }

        private string SessionFactoryAlias
        {
            get { return m_sessionFactoryAlias; }
            set { m_sessionFactoryAlias = value; }
        }

        protected ISession GetSession()
        {

            var flushMode = m_sessionManager.DefaultFlushMode;
            if (string.IsNullOrEmpty(m_sessionFactoryAlias))
            {
                //if(m_log.IsDebugEnabled)
                //    m_log.DebugFormat("Getting session with flushMode: {0}", flushMode);

                return m_sessionManager.OpenSession();
            }
            //if (m_log.IsDebugEnabled)
            //    m_log.DebugFormat("Getting session with alias: {0} and with flush mode: {1}", SessionFactoryAlias, m_sessionManager.DefaultFlushMode);
            return m_sessionManager.OpenSession(SessionFactoryAlias);
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
                    return (T) session.Get(typeof(T), id);
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
                try
                {
                    session.SaveOrUpdate(instance);
                }
                catch (Exception ex)
                {
                    throw new DataException(string.Format("Save or Update operation failed for type:{0} ", instance.GetType().Name), ex);
                }
            }
        }

        public void InitializeLazyProperties(object instance)
        {
            if (instance == null) throw new ArgumentNullException("instance");

            using (ISession session = GetSession())
            {
                foreach (object val in ReflectionUtility.GetPropertiesDictionary(instance).Values)
                {
                    if (val is INHibernateProxy || val is IPersistentCollection)
                    {
                        if (!NHibernateUtil.IsInitialized(val))
                        {
                            session.Lock(instance, LockMode.None);//lock session for this
                            NHibernateUtil.Initialize(val);
                        }
                    }
                }
            }
        }

        public virtual void SaveAll(IEnumerable data)
        {
            using (ISession session = GetSession())
            {
                foreach (var o in data)
                {
                    Save(o);
                }
            }
        }
    }

    [Transactional]
    public class NHibernateTransactionalDao : NHibernateDao
    {
        public NHibernateTransactionalDao(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public override object Create(object instance)
        {
            return base.Create(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public override void Update(object instance)
        {
            base.Update(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public override void Delete(object instance)
        {
            base.Delete(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public override void Save(object instance)
        {
            base.Save(instance);
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteAll(Type instanceType)
        {
            base.DeleteAll(instanceType);
        }

        [Transaction(TransactionMode.Requires)]
        public override void SaveAll(IEnumerable data)
        {
            base.SaveAll(data);
        }
    }



}