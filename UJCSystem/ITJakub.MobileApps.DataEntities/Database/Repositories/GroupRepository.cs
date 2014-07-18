using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.MobileApps.DataEntities.Database.Daos;
using ITJakub.MobileApps.DataEntities.Database.Entities;

namespace ITJakub.MobileApps.DataEntities.Database.Repositories
{
    [Transactional]
    public class GroupRepository: NHibernateTransactionalDao<Group>
    {
        public GroupRepository(ISessionManager sessManager): base(sessManager)
        {
        }

        public void CreateGroup(Institution institution, Task task, Group group)
        {
            group.Institution = institution;
            group.Task = task;
            Create(group);
        }
    }
}