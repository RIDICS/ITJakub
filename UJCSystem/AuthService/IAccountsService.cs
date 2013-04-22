using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using AuthService.Models;

namespace AuthService
{
    [ServiceContract]
    public interface IAccountsService
    {
        [OperationContract]
        List<User> GetAllUsers();

        [OperationContract]
        bool Authenticate(string email, string password);
        
        [OperationContract]
        bool HasRole(User user, string role);

        [OperationContract]
        string GetUserNameByEmail(string email);

        [OperationContract]
        void AddUser(User user);

        [OperationContract]
        void RemoveUser(User user);

        [OperationContract]
        void SaveContextChanges();

        [OperationContract]
        User GetUserById(int id);

        [OperationContract]
        User GetUserByEmail(string email);
    }
}
