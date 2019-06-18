using System.Collections.Generic;
using System.Reflection;
using log4net;
using Vokabular.Authentication.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.Permission;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.MainService.Core.Managers.Authentication
{
    public class PermissionConverter
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<SpecialPermissionContract> Convert(IEnumerable<PermissionContract> permissions)
        {
            var resultList = new List<SpecialPermissionContract>();
            foreach (var permission in permissions)
            {
                switch (permission.Name)
                {
                    case PermissionNames.AddNews:
                    {
                        resultList.Add(new NewsPermissionContract
                        {
                            CanAddNews = true,
                            Id = permission.Id
                        });
                        break;
                    }
                    case PermissionNames.DerivateLemmatization:
                    {
                        resultList.Add(new DerivateLemmatizationPermissionContract
                        {
                            CanDerivateLemmatization = true,
                            Id = permission.Id
                        });
                        break;
                    }
                    case PermissionNames.EditLemmatization:
                    {
                        resultList.Add(new EditLemmatizationPermissionContract
                        {
                            CanEditLemmatization = true,
                            Id = permission.Id
                        });
                        break;
                    }
                    case PermissionNames.ReadLemmatization:
                    {
                        resultList.Add(new ReadLemmatizationPermissionContract()
                        {
                            CanReadLemmatization = true,
                            Id = permission.Id
                        });
                        break;
                    }
                    case PermissionNames.EditStaticText:
                    {
                        resultList.Add(new EditStaticTextPermissionContract
                        {
                            CanEditStaticText = true,
                            Id = permission.Id
                        });
                        break;
                    }
                    case PermissionNames.EditionPrintText:
                    {
                        resultList.Add(new EditionPrintPermissionContract
                        {
                            CanEditionPrintText = true,
                            Id = permission.Id
                        });
                        break;
                    }
                    case PermissionNames.ManageFeedbacks:
                    {
                        resultList.Add(new FeedbackPermissionContract
                        {
                            CanManageFeedbacks = true,
                            Id = permission.Id
                        });
                        break;
                    }
                    case PermissionNames.ManagePermissions:
                    {
                        resultList.Add(new ManagePermissionsPermissionContract
                        {
                            CanManagePermissions = true,
                            Id = permission.Id
                        });
                        break;
                    }

                    case PermissionNames.UploadBook:
                    {
                        resultList.Add(new UploadBookPermissionContract
                        {
                            CanUploadBook = true,
                            Id = permission.Id
                        });
                        break;
                    }
                    default:
                    {
                        if (permission.Name.Contains(PermissionNames.AutoImport))
                        {
                            resultList.Add(new AutoImportCategoryPermissionContract
                            {
                                AutoImportIsAllowed = true,
                                Id = permission.Id,
                                BookType = (BookTypeEnumContract) int.Parse(permission.Name.Remove(0, PermissionNames.AutoImport.Length))
                            });
                        }
                        else if (permission.Name.Contains(PermissionNames.CardFile))
                        {
                            resultList.Add(new CardFilePermissionContract
                            {
                                CanReadCardFile = true,
                                Id = permission.Id,
                                CardFileId = permission.Name.Remove(0, PermissionNames.CardFile.Length),
                                CardFileName = permission.Description
                            });
                        }
                        else
                        {
                            string message = $"Unknown permission '{permission.Name}' with id {permission.Id}";
                            if (m_log.IsErrorEnabled)
                                m_log.Error(message);
                            //TODO throw new ArgumentOutOfRangeException(nameof(permission), permission.Name, null);
                        }                        
                        break;
                    }
                }
            }

            return resultList;
        }
    }
}