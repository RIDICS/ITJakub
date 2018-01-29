using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Works.ResourceGroup
{
    public class CreateNamedResourceGroupWork : UnitOfWorkBase<long>
    {
        private readonly ResourceRepository m_resourceRepository;
        private readonly long m_projectId;
        private readonly NamedResourceGroupContract m_newResourceGroup;

        public CreateNamedResourceGroupWork(ResourceRepository resourceRepository, long projectId, NamedResourceGroupContract newResourceGroup) : base(resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_projectId = projectId;
            m_newResourceGroup = newResourceGroup;
        }

        protected override long ExecuteWorkImplementation()
        {
            var project = m_resourceRepository.Load<Project>(m_projectId);
            var textType = Mapper.Map<TextTypeEnum>(m_newResourceGroup.TextType);
            var newNamedResourceGroup = new NamedResourceGroup
            {
                Project = project,
                Name = m_newResourceGroup.Name,
                TextType = textType,
            };
            //TODO check existing name?
            var resultId = m_resourceRepository.Create(newNamedResourceGroup);
            return (long) resultId;
        }
    }
}
