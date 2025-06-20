using EtherGizmos.Shipyard.Utilities.Extensions;
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
                : base(ErrorConstants.Code.UnsupportedOperation.QueryOptionNotApplicable,
                      target,
                      _message)
            {
            }

            public EntityNotFoundReferenceError<TEntity> AddDetail(
                Expression<Func<TEntity, object?>> selector,
                object? value)
            {
                var detail = new SelfErrorDetail(this, selector, value);
                AddDetail(detail);

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
    }
}
