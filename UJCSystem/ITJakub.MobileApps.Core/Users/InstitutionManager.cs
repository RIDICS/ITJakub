using System;
using ITJakub.MobileApps.DataEntities;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.Core.Users
{
    public class InstitutionManager
    {
        private readonly UsersRepository m_usersRepository;
        private readonly EnterCodeGenerator m_enterCodeGenerator;
        private readonly int m_maxAttemptsToSave;

        public InstitutionManager(UsersRepository usersRepository, EnterCodeGenerator enterCodeGenerator, int maxAttemptsToSave)
        {
            m_usersRepository = usersRepository;
            m_enterCodeGenerator = enterCodeGenerator;
            m_maxAttemptsToSave = maxAttemptsToSave;
        }

        public void CreateInstitution(string name)
        {
            var institution = new Institution
            {
                CreateTime = DateTime.UtcNow,
                Name = name
            };

            int attempt = 0;
            while (attempt < m_maxAttemptsToSave)
            {
                try
                {
                    institution.EnterCode = m_enterCodeGenerator.GenerateCode();
                    m_usersRepository.Create(institution);
                }
                catch (CreateEntityFailedException)
                {
                    attempt++;
                }
            }
        }

        public void AddUserToInstitution(long userId, string enterCode)
        {
            var institution = m_usersRepository.FindInstitutionByEnterCode(enterCode);
            var user = m_usersRepository.FindById<User>(userId);

            user.Institution = institution;
            m_usersRepository.Update(user);
        }
    }
}
