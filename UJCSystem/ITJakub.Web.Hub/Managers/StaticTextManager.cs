using ITJakub.Web.DataEntities.Database.Repositories;

namespace ITJakub.Web.Hub.Managers
{
    public class StaticTextManager
    {
        private readonly StaticTextRepository m_staticTextRepository;

        public StaticTextManager(StaticTextRepository staticTextRepository)
        {
            m_staticTextRepository = staticTextRepository;
        }

        public void GetText(string name)
        {
            var staticText = m_staticTextRepository.GetStaticText(name);
        }
    }
}