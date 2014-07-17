using Castle.MicroKernel;
using ITJakub.MobileApps.DataEntities.Database.Repositories;

namespace ITJakub.MobileApps.DataEntities
{
    public class StorageManager
    {
        private UserRepository m_userRepository;
        private SynchronizedObjectRepository m_synchronizedObjectRepository;
        private InstitutionRepository m_institutionRepository;
        private GroupRepository m_groupRepository;
        private TaskRepository m_taskRepository;
        private ApplicationRepository m_applicationRepository;
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
    }
}
