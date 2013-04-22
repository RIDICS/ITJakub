using AuthService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace AuthService
{

    public class AccountsService : IAccountsService
    {

        private static readonly AuthdbEntities m_container = new AuthdbEntities();

        public List<User> GetAllUsers()
        {
            var allusers = from cs in m_container.Users
                           select cs;
            return allusers.ToList();
        }

        public bool Authenticate(string email, string password)
        {
            User user = GetUserByEmail(email);
            if (user == null)
            {
                return false;
            }
            string hashedPw = SecurityUtils.HashPassword(user.email, password);
            return hashedPw.Equals(user.pwhash);
        }
        
        public bool HasRole(User user, string role)
        {
            return user.role.Equals(role);
        }

        public void AddUser(User user)
        {
            m_container.Users.Add(user);
            SaveContextChanges();
        }

        public void RemoveUser(User user)
        {
            m_container.Users.Remove(user);
            SaveContextChanges();
        }

        public void SaveContextChanges()
        {
            m_container.SaveChanges();
        }

        public User GetUserById(int id)
        {
            var user = from cs in m_container.Users
                       where cs.Id == id
                       select cs;
            User userObj = user.FirstOrDefault();
            return userObj;
        }

        public User GetUserByEmail(string email)
        {
            var user = from cs in m_container.Users
                       where cs.email == email
                       select cs;
            {
                User userObj = user.FirstOrDefault();
                return userObj;
            }
        }

        public string GetUserNameByEmail(string email)
        {
            User user = GetUserByEmail(email);
            return (user.firstname + " " + user.lastname);
        }
    }
}
