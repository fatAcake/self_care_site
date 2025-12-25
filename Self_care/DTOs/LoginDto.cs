using System.ComponentModel.DataAnnotations;

namespace Self_care.DTOs
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public string email { get; set; } = string.Empty;

        [Required]
        public string password { get; set; } = string.Empty;
    }
}
