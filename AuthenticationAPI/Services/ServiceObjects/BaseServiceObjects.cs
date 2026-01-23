using Microsoft.AspNetCore.Identity;

namespace AuthenticationAPI.Services.ServiceObjects
{
    public abstract class BaseServiceResponse
    {
        public bool Success { get; set; } = false;
        public List<string> Errors { get; set; } = new List<string>();

        public void SetErrors(string errorMessage)
        {
            Errors.Add(errorMessage);
        }

        public void SetErrors(IEnumerable<IdentityError> errors)
        {
            Errors = errors.Select(error => error.Description).ToList();
        }
    }
}
