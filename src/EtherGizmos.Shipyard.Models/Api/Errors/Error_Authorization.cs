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
                    : base(parent.Code, "authorization", string.Format(_message, GetPermission(permissionId), securableType))
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

        public class CannotGrantPermissionError : TypedErrorBase
        {
            private const string _message = "You cannot grant more permissions than you currently have.";

            public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

            public CannotGrantPermissionError()
                : base(ErrorConstants.Code.Authorization.CannotGrantPermission,
                      ErrorConstants.RequestTarget.Header,
                      _message)
            {
            }

            public CannotGrantPermissionError AddDetail(
                SecurableType securableType, int permissionId, PermissionGrantType grantType)
            {
                var detail = new SelfErrorDetail(this, securableType, permissionId, grantType);
                AddDetail(detail);

                return this;
            }

            private class SelfErrorDetail : TypedErrorDetailBase
            {
                private const string _message = "You are missing permission: {0} on {1}, with scope {2}";

                public SelfErrorDetail(
                    TypedErrorBase parent,
                    SecurableType securableType,
                    int permissionId,
                    PermissionGrantType grantType)
                    : base(parent.Code, "authorization", string.Format(_message, GetPermission(permissionId), securableType, grantType))
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

        public class CannotEditUserError : TypedErrorBase
        {
            private const string _message = "You cannot edit a user that has more permissions than you currently have.";

            public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

            public CannotEditUserError()
                : base(ErrorConstants.Code.Authorization.CannotEditUser,
                      ErrorConstants.RequestTarget.Header,
                      _message)
            {
            }

            public CannotEditUserError AddDetail(
                SecurableType securableType, int permissionId, PermissionGrantType grantType)
            {
                var detail = new SelfErrorDetail(this, securableType, permissionId, grantType);
                AddDetail(detail);

                return this;
            }

            private class SelfErrorDetail : TypedErrorDetailBase
            {
                private const string _message = "You are missing permission: {0} on {1}, with scope {2}";

                public SelfErrorDetail(
                    TypedErrorBase parent,
                    SecurableType securableType,
                    int permissionId,
                    PermissionGrantType grantType)
                    : base(parent.Code, "authorization", string.Format(_message, GetPermission(permissionId), securableType, grantType))
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
