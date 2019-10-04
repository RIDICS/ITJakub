﻿namespace Vokabular.MainService.DataContracts.Contracts.Permission
{
    public class PermissionContract
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class PermissionDataContract
    {
        public bool ShowPublished { get; set; }
        public bool ReadProject { get; set; }
        public bool EditProject { get; set; }
        public bool AdminProject { get; set; }
    }
}
