using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using log4net;
using NHibernate;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.DataEntities.Database.Daos
{
    public class NHibernateDao
    {
        protected const string WildcardAny = "%";
        protected const string WildcardSingle = "_";

        private readonly IUnitOfWork m_unitOfWork;

        protected static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public NHibernateDao(IUnitOfWork unitOfWork)
        {
            m_unitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork
        {
            get { return m_unitOfWork; }
        }

        protected ISession GetSession()
        {
            return m_unitOfWork.CurrentSession;
        }

        public virtual object FindById(Type type, object id)
        {
            try
            {
                return GetSession().Get(type, id);
            }
            catch (Exception ex)
            {
                throw new DataException(string.Format("Get by id operation failed for type:{0}", type.Name), ex);
            }
        }

        public virtual T FindById<T>(object id)
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

        public virtual object Load(Type type, object id)
        {
            try
            {
                return GetSession().Load(type, id);
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
        public virtual T Load<T>(object id)
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

        public virtual object Create(object instance)
        {
            try
            {
                return GetSession().Save(instance);
            }
            catch (Exception ex)
            {
                throw new DataException(string.Format("Create operation failed for type:{0}", instance.GetType().Name), ex);
            }
        }

        public virtual IList<object> CreateAll(IEnumerable data)
        {
            var result = new List<object>();
            foreach (var instance in data)
            {
                try
                {
                    var id = GetSession().Save(instance);
                    result.Add(id);
                }
                catch (Exception ex)
                {
                    throw new DataException(string.Format("Create operation failed for type:{0}", instance.GetType().Name), ex);
                }
            }
            return result;
        }

        public virtual void Delete(object instance)
        {
            try
            {
                GetSession().Delete(instance);
            }
            catch (Exception ex)
            {
                throw new DataException(string.Format("Delete operation failed for type:{0}", instance.GetType().Name), ex);
            }
        }

        public virtual void Update(object instance)
        {
            try
            {
                GetSession().Update(instance);
            }
            catch (Exception ex)
            {
                throw new DataException(string.Format("Update operation failed for type:{0}", instance.GetType().Name), ex);
            }
        }

        public virtual void DeleteAll(Type type)
        {
            try
            {
                GetSession().Delete(String.Format("from {0}", type.Name));
            }
            catch (Exception ex)
            {
                throw new DataException(string.Format("Delete operation failed for type:{0}", type.Name), ex);
            }
        }

        protected virtual void Save(object instance)
        {
            try
            {
                GetSession().SaveOrUpdate(instance);
            }
            catch (Exception ex)
            {
                throw new DataException(string.Format("Save or Update operation failed for type:{0} ", instance.GetType().Name), ex);
            }
        }

        public virtual void SaveAll(IEnumerable data)
        {
            foreach (var o in data)
            {
                Save(o);
            }
        }
    }
}