using System.ServiceModel;

namespace ITJakub.ITJakubService.DataContracts
{
    [ServiceContract]
    public interface IItJakubServiceUnauthorized
    {
        [OperationContract]
        UserContract FindUserById(int userId);
        
        [OperationContract]
        UserContract FindUserByUserName(string userName);

        [OperationContract]
        UserContract CreateUser(UserContract user);
    }
}