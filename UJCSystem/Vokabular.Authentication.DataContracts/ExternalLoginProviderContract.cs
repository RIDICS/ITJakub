using System.ComponentModel.DataAnnotations;

namespace Vokabular.Authentication.DataContracts
{
    public class ExternalLoginProviderContract
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public bool Enable { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string AuthenticationScheme { get; set; }

        public bool DisableManagingByUser { get; set; }

        public int LogoResourceId { get; set; }

        public string MainColor { get; set; }

        public string Description { get; set; }

        public string DescriptionLocalizationKey { get; set; }
    }
}
