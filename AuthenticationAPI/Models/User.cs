using Microsoft.AspNetCore.Identity;

namespace AuthenticationAPI.Models
{
    public class User : IdentityUser
    {
        public DateTime? CreatedDate { get; set; } = null;
    }
}
