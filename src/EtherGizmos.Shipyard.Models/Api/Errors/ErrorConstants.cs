namespace EtherGizmos.Shipyard.Models.Api.Errors;

public static class ErrorConstants
{
    public static class RequestTarget
    {
        public const string Body = "request.body";

        public const string Header = "request.header";

        public const string Method = "request.method";

        public const string Query = "request.query";

        public const string Uri = "request.uri";
    }

    public static class Code
    {
        public static class Source
        {
            public const string Client = "client";

            public const string Server = "server";
        }

        public static class Authentication
        {
            public const string Root = "authentication";

            public const string MissingToken = $"{Source.Client}.{Root}.missingToken";
        }

        public static class Reference
        {
            public const string Root = "reference";

            public const string EntityNotFound = $"{Source.Client}.{Root}.entityNotFound";

            public const string InvalidKeyType = $"{Source.Client}.{Root}.invalidKeyType";
        }

        public static class UnsupportedOperation
        {
            public const string Root = "unsupportedOperation";

            public const string QueryOptionNotApplicable = $"{Source.Client}.{Root}.queryOptionNotApplicable";
        }

        public static class Validation
        {
            public const string Root = "validation";

            public const string ModelStateInvalid = $"{Source.Client}.{Root}.modelStateInvalid";
        }
    }
}
