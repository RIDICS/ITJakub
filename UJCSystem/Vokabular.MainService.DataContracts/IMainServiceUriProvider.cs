using System;

namespace Vokabular.MainService.DataContracts
{ 
    public interface IMainServiceUriProvider
    {
        Uri MainServiceUri { get; set; }
    }
}
