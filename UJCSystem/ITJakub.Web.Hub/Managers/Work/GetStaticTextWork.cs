using ITJakub.Web.DataEntities.Database.Entities;
using ITJakub.Web.DataEntities.Database.Repositories;
using ITJakub.Web.DataEntities.Database.UnitOfWork;

namespace ITJakub.Web.Hub.Managers.Work
{
    public class GetStaticTextWork : UnitOfWorkBase<StaticText>
    {
        private readonly StaticTextRepository m_staticTextRepository;
        private readonly string m_staticTextName;

        public GetStaticTextWork(StaticTextRepository staticTextRepository, string staticTextName) : base(staticTextRepository.UnitOfWork)
        {
            m_staticTextRepository = staticTextRepository;
            m_staticTextName = staticTextName;
        }

        protected override StaticText ExecuteWorkImplementation()
        {
            return m_staticTextRepository.GetStaticText(m_staticTextName);
        }
    }
}