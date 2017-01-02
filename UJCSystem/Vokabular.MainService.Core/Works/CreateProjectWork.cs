using Vokabular.DataEntities.Database.UnitOfWork;

namespace Vokabular.MainService.Core.Works
{
    public class CreateProjectWork : UnitOfWorkBase
    {
        public CreateProjectWork(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        protected override void ExecuteWorkImplementation()
        {
            throw new System.NotImplementedException();
        }
    }
}
