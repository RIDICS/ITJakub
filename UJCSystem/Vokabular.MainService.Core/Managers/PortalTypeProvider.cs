using System;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.Managers
{
    public class PortalTypeProvider
    {
        public PortalTypeContract PortalType { get; set; }

        public ProjectTypeContract GetDefaultProjectType()
        {
            switch (PortalType)
            {
                case PortalTypeContract.Research:
                    return ProjectTypeContract.Research;
                case PortalTypeContract.Community:
                    return ProjectTypeContract.Community;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
