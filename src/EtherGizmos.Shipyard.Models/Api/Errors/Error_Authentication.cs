using EtherGizmos.Shipyard.Api.Errors;
using System.Net;

namespace EtherGizmos.Shipyard.Models.Api.Errors;

partial class Error
{
    public static class Authentication
    {
        public class MissingToken : TypedErrorBase
        {
            private const string _message = "Missing an Authorization header.";

            public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;

            public MissingToken()
                : base(ErrorConstants.Code.Authentication.MissingToken,
                      ErrorConstants.RequestTarget.Header,
                      _message)
            {
            }
        }
    }
}
