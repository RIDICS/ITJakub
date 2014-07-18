using Castle.MicroKernel;
using ITJakub.MobileApps.DataEntities.Database.Entities;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.DataEntities
{
    public class StorageManager
    {
        private readonly UserRepository m_userRepository;
        private SynchronizedObjectRepository m_synchronizedObjectRepository;
        private readonly InstitutionRepository m_institutionRepository;
        private readonly GroupRepository m_groupRepository;
        private readonly TaskRepository m_taskRepository;
        private readonly ApplicationRepository m_applicationRepository;
        private RoleRepository m_roleRepository;

        public StorageManager(IKernel container)
        {
            m_userRepository = container.Resolve<UserRepository>();
            m_synchronizedObjectRepository = container.Resolve<SynchronizedObjectRepository>();
            m_institutionRepository = container.Resolve<InstitutionRepository>();
            m_groupRepository = container.Resolve<GroupRepository>();
            m_taskRepository = container.Resolve<TaskRepository>();
            m_applicationRepository = container.Resolve<ApplicationRepository>();
            m_roleRepository = container.Resolve<RoleRepository>();
        }

        public Institution FindInstitutionById(long id)
        {
            return m_institutionRepository.FindById(id);
        }


        public void CreateUser(User user)
        {
            m_userRepository.CreateUser(user);
        }

        public User FindUserById(long id)
        {   
            return m_userRepository.FindById(id);
        }

        public Application FindApplicationById(long id)
        {
            return m_applicationRepository.FindById(id);
        }

        public void CreateTask(Application application, string data)
        {
            m_taskRepository.Create(new Task() {Application = application}); //TODO add Data
        }

        public void CreateGroup(Institution institution, Task task, Group group)
        {
            m_groupRepository.CreateGroup(institution, task, group);
        }

        public Task FindTaskById(long id)
        {
            return m_taskRepository.FindById(id);
        }

        public Group FindGroupById(long id)
        {
            return m_groupRepository.FindById(id);
        }
    }
}
