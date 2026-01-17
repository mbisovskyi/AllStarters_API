namespace AuthenticationAPI.Controllers.ControllerObjects.ControllerBaseResponseObjects
{
    public class ControllerFailedResponse
    {
        public bool Success { get; set; } = false;
        public string Status { get; set; } = string.Empty;
    }

    public class ControllerSuccessResponse
    {
        public bool Success { get; set; } = true;
        public string Status { get; set; } = string.Empty;
    }
}
