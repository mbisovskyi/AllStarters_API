using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace AuthenticationAPI.Services.ServiceObjects
{
    public abstract class BaseServiceResponse : ControllerBase // Inherits ControllerBase to be able to return IActionResult objects
    {
        private IActionResult Result { get; set; } = new EmptyResult();

        public IActionResult GetResult()
        {
            return Result;
        }

        public void SetOkResult()
        {
            Result = Ok();
        }

        public void SetOkResult(object data)
        {
            Result = Ok(new { Data = data });
        }

        public void SetOkResult(string message)
        {
            Result = Ok(new { Data = new { Message = message }});
        }

        public void SetBadRequestResult()
        { 
            Result = BadRequest();
        }

        public void SetBadRequestResult(object data)
        {
            Result = BadRequest(new { Data = data });
        }

        public void SetBadRequestResult(string message)
        {
            Result = BadRequest(new { Data = new { Message = message }});
        }
    }
}
