using System;

namespace Vokabular.DataEntities.Database.Entities.Enums
{
    [Flags]
    public enum PermissionFlag : int
    {
        None = 0x00,
        ShowPublished = 0x01,
        ReadProject = 0x02,
        EditProject = 0x04,
        AdminProject = 0x08,
    }
}