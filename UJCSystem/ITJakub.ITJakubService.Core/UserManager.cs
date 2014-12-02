using System;
using System.Data;
using ITJakub.DataEntities.Database.Entities;
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

        public CreateUserResultContract CreateUser(CreateUserContract userInfo)
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
    }
}