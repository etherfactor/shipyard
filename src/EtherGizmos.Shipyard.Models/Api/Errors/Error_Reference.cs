using EtherGizmos.Common.Utilities.Extensions;
using System.Linq.Expressions;
using System.Net;

namespace EtherGizmos.Shipyard.Models.Api.Errors;

public partial class Error
{
    public static class Reference
    {
        public class EntityNotFoundReferenceError<TEntity> : TypedErrorBase
        {
            private const string _message = "An entity with the provided id was not found.";

            public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

            public EntityNotFoundReferenceError(
                string target = ErrorConstants.RequestTarget.Uri)
                : base(ErrorConstants.Code.Reference.EntityNotFound,
                    target,
                    _message)
            {
            }

            public EntityNotFoundReferenceError<TEntity> AddDetail(
                params (Expression<Func<TEntity, object?>> Selector, object? Value)[] keys)
            {
                foreach (var (selector, value) in keys)
                {
                    var detail = new SelfErrorDetail(this, selector, value);
                    AddDetail(detail);
                }

                return this;
            }

            private class SelfErrorDetail : TypedErrorDetailBase
            {
                private const string _message = "Non-existent key value: {0}";

                public SelfErrorDetail(
                    TypedErrorBase parent,
                    Expression<Func<TEntity, object?>> selector,
                    object? value)
                    : base(parent.Code, selector.GetPath(), string.Format(_message, value))
                {
                }
            }
        }

        /// <summary>
        /// Thrown when the value provided in the URL cannot be converted into the expected key type.
        /// </summary>
        public class InvalidKeyTypeReferenceError<TEntity> : TypedErrorBase
        {
            private const string _message = "The provided key value is not a valid format for this entity.";

            private readonly string _target;

            public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

            public InvalidKeyTypeReferenceError(
                string target = ErrorConstants.RequestTarget.Uri)
                : base(ErrorConstants.Code.Reference.InvalidKeyType,
                    target,
                    _message)
            {
                _target = target;
            }

            /// <summary>
            /// Adds one or more details about which keys failed to parse.
            /// </summary>
            public InvalidKeyTypeReferenceError<TEntity> AddDetail(
                params (Type Type, object RawValue)[] keys)
            {
                foreach (var (type, rawValue) in keys)
                {
                    var target = _target switch
                    {
                        ErrorConstants.RequestTarget.Query => "$id",
                        ErrorConstants.RequestTarget.Body => "@odata.id",
                        _ => throw new NotSupportedException()
                    };

                    AddDetail(new KeyParsingErrorDetail(this, target, type.Name, rawValue));
                }

                return this;
            }

            private class KeyParsingErrorDetail : TypedErrorDetailBase
            {
                private const string _detailMessage = "Could not parse key value '{0}' as type {1}.";

                public KeyParsingErrorDetail(
                    TypedErrorBase parent,
                    string target,
                    string type,
                    object? rawValue)
                    : base(parent.Code,
                        target,
                        string.Format(_detailMessage, rawValue, type))
                {
                }
            }
        }
    }
}
