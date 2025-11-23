using EtherGizmos.Shipyard.Database.Enums;
using System.Net;

namespace EtherGizmos.Shipyard.Api.Errors;

partial class Error
{
    public static class Authorization
    {
        public class MissingPermissionError : TypedErrorBase
        {
            private const string _message = "You lack the necessary permissions to perform this action.";

            public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

            public MissingPermissionError()
                : base(ErrorConstants.Code.Authorization.MissingPermission,
                      ErrorConstants.RequestTarget.Header,
                      _message)
            {
            }

            public MissingPermissionError AddDetail(
                SecurableType securableType, int permissionId)
            {
                var detail = new SelfErrorDetail(this, securableType, permissionId);
                AddDetail(detail);

                return this;
            }

            private class SelfErrorDetail : TypedErrorDetailBase
            {
                private const string _message = "Missing required permission: {0} on {1}";

                public SelfErrorDetail(
                    TypedErrorBase parent,
                    SecurableType securableType,
                    int permissionId)
                    : base(parent.Code, "$self", string.Format(_message, GetPermission(permissionId), securableType))
                {
                }

                private static string GetPermission(
                    int permissionId)
                    => permissionId switch
                    {
                        1 => "Read",
                        2 => "Write",
                        4 => "Delete",
                        _ => "Unknown"
                    };
            }
        }
    }
}
