using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace DocProjectStorageWeb.WebModels
{

    public enum Role
    {
        REDACTOR,
        TECHNICAL_REDACTOR
    }

    public enum ProjectType
    {
        POETRY, PROSE, DICTIONARY, SECONDARY_LITERATURE
    }

    public class RoleSelection
    {
        public string RoleName;
        public bool IsSet;
    }

    public enum ProjectState
    {
        [Display(Name = "U editora")]
        EDITOR,
        [Display(Name = "Pro redaktory")]
        FOR_REDACTORS,
        [Display(Name = "U redaktora")]
        REDACTOR,
        [Display(Name = "Pro tech. redaktory")]
        FOR_TECH_REDACTORS,
        [Display(Name = "U tech. redaktora")]
        TECH_REDACTOR,
        [Display(Name = "Publikováno")]
        PUBLISHED
    }

    public enum EventType
    {
        [Display(Name = "Projekt změnil stav")]
        PROJECT_CHANGED_STATE,
        [Display(Name = "Nové zprávy v diskuzi")]
        PROJECT_DISCUSSION_UPDATED,
        [Display(Name = "Pozvánka do projektu")]
        INVITED_TO_PROJECT,
        [Display(Name = "Vyřazení z projektu")]
        REMOVED_FROM_PROJECT
    }

    public class LoginModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [Display(Name = "Zapamatuj si mě?")]
        public bool RememberMe { get; set; }
    }

    public class DashboardModel
    {
        public List<DocProjectStorageWeb.Models.DocProjectEntity> Projects { get; set; }

        public List<DocProjectStorageWeb.Models.EventEntity> LastActivities { get; set; }

        public List<DocProjectStorageWeb.Models.EventEntity> UserActivities { get; set; }
    }

    public class NewProjectModel
    {
        [Required]
        [Display(Name = "Název")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Required]
        [Display(Name = "Typ dokumentu")]
        public string DocTypeValue{ get; set; }

        [Required]
        [Display(Name = "DocumentType")]
        public SelectListItem[] DocTypes { get; set; }
    }

    public class ProjectModel
    {
        [Required]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Název")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Required]
        [Display(Name = "Typ dokumentu")]
        public string DocType { get; set; }

        [Required]
        [Display(Name = "Editoři")]
        public ICollection<string> Editors { get; set; }

        [Required]
        [Display(Name = "Zodpovědní redaktoři")]
        public ICollection<string> ResponsibleRedactors { get; set; }

        [Required]
        [Display(Name = "Zodpovědní tech. redaktoři")]
        public ICollection<string> ResponsibleTechRedactors { get; set; }

        [Required]
        [Display(Name = "Revize")]
        public ICollection<Revision> Revisions { get; set; }

        [Required]
        [Display(Name = "Stav projektu")]
        public ProjectState ProjectState { get; set; }

        [Required]
        [Display(Name = "Osoba mající zámek")]
        public string LockingPerson { get; set; }

        [Required]
        [Display(Name = "Diskuze")]
        public ICollection<Message> Messages { get; set; }
    }

    public class Revision
    {
        [Required]
        [Display(Name = "Číslo revize")]
        public int RevNumber { get; set; }

        [Required]
        [Display(Name = "Datum revize")]
        public DateTime ReleaseDate { get; set; }
    }

    public class Message
    {
        [Required]
        [Display(Name = "Odesláno")]
        public DateTime SentTime { get; set; }

        [Required]
        [Display(Name = "Obsah zprávy")]
        public string Content { get; set; }

        [Required]
        [Display(Name = "Odesilatel")]
        public string Sender { get; set; }
    }

    public class UserRolesModel
    {
        public string UserName { get; set; }

        public ICollection<RoleSelection> Roles { get; set; }
    }

    public class SetupModel
    {
        public ICollection<string> DocumentTypeNames { get; set; }
    }

    public class UserModel
    {
        public string Name { get; set; }
    }
}
