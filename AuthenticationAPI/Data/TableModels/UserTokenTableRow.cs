using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationAPI.Data.TableModels
{
    public class UserTokenTableRow : IdentityUserToken<string>
    {
        [Required]
        public bool LifetimeExtended { get; set; }
    }
}
