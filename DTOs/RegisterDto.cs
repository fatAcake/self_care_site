using System.ComponentModel.DataAnnotations;

namespace Self_care.DTOs
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string password { get; set; } = string.Empty;

        [Required]
        public string nickname { get; set; } = string.Empty;

        public string timezone { get; set; } = "UTC";
    }
}
