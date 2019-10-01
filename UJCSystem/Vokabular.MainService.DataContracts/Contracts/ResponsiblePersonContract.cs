﻿namespace Vokabular.MainService.DataContracts.Contracts
{
    public class ResponsiblePersonContract
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ProjectResponsiblePersonContract : ResponsiblePersonContract
    {
        public int Sequence { get; set; }
        public ResponsibleTypeContract ResponsibleType { get; set; }
    }

    public class ProjectResponsiblePersonIdContract
    {
        public int ResponsiblePersonId { get; set; }
        public int ResponsibleTypeId { get; set; }
    }
}