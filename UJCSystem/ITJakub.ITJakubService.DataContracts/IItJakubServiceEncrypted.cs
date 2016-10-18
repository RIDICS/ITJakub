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
        PrivateUserContract PrivateFindUserById(int userId);

        [OperationContract]
        UserContract FindUserByUserName(string userName);
        
        [OperationContract]
        PrivateUserContract PrivateFindUserByUserName(string userName);

        [OperationContract]
        PrivateUserContract CreateUser(PrivateUserContract user);

        [OperationContract]
        bool RenewCommToken(string username);

        #endregion


    }
}