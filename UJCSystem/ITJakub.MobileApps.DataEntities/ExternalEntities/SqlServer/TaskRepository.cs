using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using ITJakub.MobileApps.DataEntities.ExternalEntities.SqlServer.Entities;

namespace ITJakub.MobileApps.DataEntities.ExternalEntities.SqlServer
{
    [Transactional]
    public class TaskRepository : NHibernateTransactionalDao, ITaskDao
    {
        public TaskRepository(ISessionManager sessManager) : base(sessManager)
        {
        }


        public virtual ITaskEntity GetNewEntity(long taskId, int appId, string data)
        {
            using (var session = GetSession())
            {
                return new TaskData {Task = session.Load<Task>(taskId), Application = session.Load<Application>(appId), Data = data};
            }
        }

        public virtual void Save(ITaskEntity taskEntity)
        {
            base.Save(taskEntity);
        }

        [Transaction]
        public virtual ITaskEntity FindByIdAndAppId(long taskId, int appId)
        {
            using (var session = GetSession())
            {
                return session.QueryOver<TaskData>().Where(x => x.Task.Id == taskId).SingleOrDefault<TaskData>();
            }
        }
    }
}