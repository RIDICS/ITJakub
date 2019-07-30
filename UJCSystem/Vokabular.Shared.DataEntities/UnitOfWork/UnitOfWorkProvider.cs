using System.Collections.Generic;
using System.Linq;

namespace Vokabular.Shared.DataEntities.UnitOfWork
{
    public class UnitOfWorkProvider
    {
        private readonly IList<KeyValuePair<object, IUnitOfWork>> m_allUnitOfWorks;

        public UnitOfWorkProvider(IList<KeyValuePair<object, IUnitOfWork>> allUnitOfWorks)
        {
            m_allUnitOfWorks = allUnitOfWorks;
        }

        public IUnitOfWork GetUnitOfWork(string serviceKey)
        {
            return m_allUnitOfWorks.FirstOrDefault(x => x.Key as string == serviceKey).Value ??
                   m_allUnitOfWorks.FirstOrDefault(x => x.Key == null).Value;
        }
    }
}