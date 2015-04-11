using System;
using System.Data;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core
{
    public class UserManager
    {
        private readonly UserRepository m_userRepository;

        public UserManager(UserRepository userRepository)
        {
            m_userRepository = userRepository;
        }

        public CreateUserResultContract CreateUser(CreateUserContract userInfo) //TODO delete this method
        {
            try
            {
                if (userInfo.AuthenticationProvider == AuthProviderEnumContract.ItJakub)  //TODO should be dicitonary of RegisterProviders and select right one by enum as key
                {
                    m_userRepository.RegisterLocalAccount(userInfo.Email, userInfo.Password, userInfo.FirstName,
                        userInfo.LastName);
                }
                else
                {
                    throw new NotImplementedException();
                }
                return new CreateUserResultContract {Successfull = true};
            }
            catch (DataException ex)
            {
                return new CreateUserResultContract {Successfull = false};
            }
        }

        public LoginUserResultContract LoginUser(LoginUserContract userInfo)
        {
            User user;
            if (userInfo.AuthenticationProvider == AuthProviderEnumContract.ItJakub)    //TODO should be dicitonary of LoginProviders and select right one by enum as key
            {
                var userI = m_userRepository.LoginUserWithLocalAccount(userInfo.Email, userInfo.Password);
                user = userI;
            }
            else
            {
                throw new NotImplementedException();
            }
            
            if (user != null)
            {
                return new LoginUserResultContract {Successfull = true, CommunicationToken = user.CommunicationToken};
            }
            return new LoginUserResultContract {Successfull = false};
        }



        #region New auth methods

        public UserContract CreateLocalUser(UserContract user) //TODO write automapper profiles
        {
            var now = DateTime.UtcNow;
            var dbUser = new User
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreateTime = now,
                PasswordHash = user.PasswordHash,
                AuthenticationProvider = AuthenticationProvider.ItJakub,
                CommunicationToken = Guid.NewGuid().ToString(), //TODO remove token
                CommunicationTokenCreateTime = now,
            };
            var userId = m_userRepository.Create(dbUser);
            return FindById(userId);
        }

        public UserContract FindByUserName(string userName)
        {
            var dbUser = m_userRepository.FindByUserName(userName);
            if (dbUser == null) return null;
            var user = new UserContract
            {
                Id = dbUser.Id,
                UserName = dbUser.UserName,
                Email = dbUser.Email,
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                CreateTime = dbUser.CreateTime,
                PasswordHash = dbUser.PasswordHash
            };
            return user;
        }

        public UserContract FindById(int userId)
        {
            var dbUser = m_userRepository.FindById(userId);
            if (dbUser == null) return null;
            var user = new UserContract
            {
                Id = dbUser.Id,
                UserName = dbUser.UserName,
                Email = dbUser.Email,
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                CreateTime = dbUser.CreateTime,
                PasswordHash = dbUser.PasswordHash
            };
            return user;
        }


        #endregion
    }
}