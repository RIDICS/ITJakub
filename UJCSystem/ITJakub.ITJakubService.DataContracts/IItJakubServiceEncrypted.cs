using System.ServiceModel;
using ITJakub.Shared.Contracts;

namespace ITJakub.ITJakubService.DataContracts
{
    [ServiceContract]
    public interface IItJakubServiceEncrypted
    {
        #region User Operations
        [OperationContract]
        UserContract FindUserById(int userId);
        
        [OperationContract]
        UserContract FindUserByUserName(string userName);

        [OperationContract]
        UserContract CreateUser(UserContract user);
        #endregion

       
    }
}