using System.Net;
using System.Runtime.Serialization;

namespace GKTodoManager.Domain.Base
{
    public class ErrorResponse
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public HttpStatusCode Status { get; set; }
    }
}
