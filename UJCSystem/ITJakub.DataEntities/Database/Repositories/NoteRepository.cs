using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;

namespace ITJakub.DataEntities.Database.Repositories
{
    [Transactional]
    public class NoteRepository : NHibernateTransactionalDao<Note>
    {
        public NoteRepository(ISessionManager sessManager) : base(sessManager)
        {
        }
    }
}