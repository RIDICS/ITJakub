using System.Collections.Generic;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.FileProcessing.Service.Test.Mock
{
    public class MockUnitOfWorkProvider
    {
        public static UnitOfWorkProvider Create()
        {
            var unitOfWork = new MockUnitOfWork();
            var unitOfWorkProvider = new UnitOfWorkProvider(new List<KeyValuePair<object, IUnitOfWork>>
            {
                new KeyValuePair<object, IUnitOfWork>(null, unitOfWork)
            });

            return unitOfWorkProvider;
        }
    }
}