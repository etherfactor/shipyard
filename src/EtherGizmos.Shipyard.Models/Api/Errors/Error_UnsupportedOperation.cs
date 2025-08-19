using EtherGizmos.Shipyard.Api.Errors;
using System.Net;

namespace EtherGizmos.Shipyard.Models.Api.Errors;

partial class Error
{
    public static class UnsupportedOperation
    {
        public class QueryOptionNotApplicable : TypedErrorBase
        {
            private const string _message = "The provided query parameter is not applicable for single records.";

            public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

            public QueryOptionNotApplicable()
                : base(ErrorConstants.Code.UnsupportedOperation.QueryOptionNotApplicable,
                      ErrorConstants.RequestTarget.Query,
                      _message)
            {
            }

            public QueryOptionNotApplicable AddDetail(
                string parameterName)
            {
                var detail = new SelfErrorDetail(this, parameterName);
                AddDetail(detail);

                return this;
            }

            private class SelfErrorDetail : TypedErrorDetailBase
            {
                private const string _message = "The query option {0} cannot be applied to a single entity request.";

                public SelfErrorDetail(
                    TypedErrorBase parent, string parameterName)
                    : base(parent.Code, parameterName, string.Format(_message, parameterName))
                {
                }
            }
        }
    }
}
