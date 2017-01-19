using System;
using AutoMapper;
using ITJakub.Web.DataEntities.Database.Entities;
using ITJakub.Web.DataEntities.Database.Entities.Enums;
using ITJakub.Web.DataEntities.Database.Repositories;
using ITJakub.Web.DataEntities.Database.UnitOfWork;
using ITJakub.Web.Hub.Models;

namespace ITJakub.Web.Hub.Managers.Work
{
    public class SaveStaticTextWork : UnitOfWorkBase<ModificationUpdateViewModel>
    {
        private readonly StaticTextRepository m_staticTextRepository;
        private readonly string m_staticTextName;
        private readonly string m_text;
        private readonly StaticTextFormat m_format;
        private readonly string m_username;

        public SaveStaticTextWork(StaticTextRepository staticTextRepository, string staticTextName, string text, StaticTextFormat format, string username) : base(staticTextRepository.UnitOfWork)
        {
            m_staticTextRepository = staticTextRepository;
            m_staticTextName = staticTextName;
            m_text = text;
            m_format = format;
            m_username = username;
        }

        protected override ModificationUpdateViewModel ExecuteWorkImplementation()
        {
            var now = DateTime.UtcNow;
            var staticTextEntity = m_staticTextRepository.GetStaticText(m_staticTextName);
            if (staticTextEntity == null)
            {
                staticTextEntity = new StaticText
                {
                    Name = m_staticTextName
                };
            }

            staticTextEntity.Text = m_text;
            staticTextEntity.Format = m_format;
            staticTextEntity.ModificationUser = m_username;
            staticTextEntity.ModificationTime = now;

            m_staticTextRepository.Save(staticTextEntity);

            return Mapper.Map<ModificationUpdateViewModel>(staticTextEntity);
        }
    }
}