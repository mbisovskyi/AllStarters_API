using System.ComponentModel.DataAnnotations;

namespace AuthenticationAPI.DTO
{
    public class RegisterAccountDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string UserRole { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
    }
}
