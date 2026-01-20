using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthenticationAPI.Services.ServiceObjects
{
    public abstract class BaseServiceResponse : ControllerBase // Inherits ControllerBase to be able to return IActionResult objects
    {
        private IActionResult Result { get; set; } = new EmptyResult();

        public IActionResult GetResult()
        {
            return Result;
        }

        public void SetOk()
        {
            Result = Ok();
        }

        public void SetOk(object data)
        {
            Result = Ok(new { Data = data });
        }

        public void SetOk(string message)
        {
            Result = Ok(new { Data = new { Message = message }});
        }

        public void SetBadRequestErrors(IDictionary<string, List<string>> errors)
        {
            Result = BadRequest(new { Errors = errors });
        }

        public void SetBadRequestErrors(object errors)
        {
                Result = BadRequest(new { Errors =  errors});
        }

        public void SetBadRequestErrors(IEnumerable<IdentityError> errors)
        {
            List<string> parsedErrors = parseIdentityErrors(errors);
            Result = BadRequest(new { Errors = parsedErrors });
        }

        public void SetBadRequestErrors(string message)
        {
            Result = BadRequest(new { Errors = new { Status = message }});
        }

        // Class Helpers
        private List<string> parseIdentityErrors(IEnumerable<IdentityError> errors)
        {
            return errors.Select(error => error.Description).ToList();
        }
    }
}
