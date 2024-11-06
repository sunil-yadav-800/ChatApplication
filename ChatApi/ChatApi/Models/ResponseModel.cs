using System.Security.Principal;

namespace ChatApi.Models
{
    public class ResponseModel
    {
        public ResponseModel()
        {
            status = false;
            message = string.Empty;
            data = null;
        }
        public bool status { get; set; }
        public string message { get; set; }
        public object? data { get; set; }
    }
}
