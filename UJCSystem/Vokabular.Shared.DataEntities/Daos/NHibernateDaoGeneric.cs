using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using log4net;
using NHibernate;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.Shared.DataEntities.Daos
{
    public class NHibernateDao<T> where T : IEquatable<T>
    {
        private readonly IUnitOfWork m_unitOfWork;

        protected static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public NHibernateDao(IUnitOfWork unitOfWork)
        {
            m_unitOfWork = unitOfWork;
        }

        protected IUnitOfWork UnitOfWork
        {
            get { return m_unitOfWork; }
        }

        protected ISession GetSession()
        {
            return m_unitOfWork.CurrentSession;
        }

        public virtual T FindById(object id)
        {
            try
            {
                return (T)GetSession().Get(typeof(T), id);
            }
            catch (Exception ex)
            {
                throw new DataException(string.Format("Get by id operation failed for type:{0}", typeof(T).Name), ex);
            }
        }

        public virtual T Load(object id)
        {
            try
            {
                return (T)GetSession().Load(typeof(T), id);
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

        /// <summary>
        /// returning primary key
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual object Create(T instance)
        {
            try
            {
                return GetSession().Save(instance);
            }
            catch (Exception ex)
            {
                throw new DataException(
                    string.Format("Create operation failed for type:{0}", instance.GetType().Name), ex);
            }
        }

        public virtual object Create(object instance)
        {
            try
            {
                return GetSession().Save(instance);
            }
            catch (Exception ex)
            {
                throw new DataException(
                    string.Format("Create operation failed for type:{0}", instance.GetType().Name), ex);
            }
        }

        public virtual void Delete(T instance)
        {
            try
            {
                GetSession().Delete(instance);
            }
            catch (Exception ex)
            {
                throw new DataException(
                    string.Format("Delete operation failed for type:{0}", instance.GetType().Name), ex);
            }
        }

        public virtual void DeleteAll()
        {
            try
            {
                GetSession().Delete(string.Format("from {0}", typeof(T).Name));
            }
            catch (Exception ex)
            {
                throw new DataException(string.Format("Delete operation failed for type:{0}", typeof(T).Name), ex);
            }
        }

        public virtual void Update(T instance)
        {
            Update((object)instance);
        }

        public virtual void Update(object instance)
        {
            try
            {
                GetSession().Update(instance);
            }
            catch (Exception ex)
            {
                throw new DataException(
                    string.Format("Update operation failed for type:{0}", instance.GetType().Name), ex);
            }
        }

        public virtual void Save(object instance)
        {
            try
            {
                GetSession().SaveOrUpdate(instance);
            }
            catch (Exception ex)
            {
                throw new DataException(
                    string.Format("Save or Update operation failed for type:{0} ", instance.GetType().Name), ex);
            }
        }

        public virtual void Save(T instance)
        {
            Save((object)instance);
        }
        
        public virtual void SaveAll(IEnumerable<T> data)
        {
            foreach (T o in data)
            {
                Save(o);
            }
        }

        public virtual void SaveAll(IEnumerable data)
        {
            foreach (object o in data)
            {
                Save(o);
            }
        }
    }
}