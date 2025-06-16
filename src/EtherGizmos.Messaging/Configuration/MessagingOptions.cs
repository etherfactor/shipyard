namespace EtherGizmos.Messaging.Configuration;

public class MessagingOptions
{
    public bool Serverless { get; set; }

    public Dictionary<string, MessagePublisherOptions> Publishers { get; set; } = [];

    public Dictionary<string, MessageListenerOptions> Listeners { get; set; } = [];

    public Dictionary<Type, string> TypeMappings { get; set; } = [];

    internal Dictionary<string, Type> ReverseTypeMappings { get; set; } = [];

    internal void Build()
    {
        ReverseTypeMappings = TypeMappings
            .Select(e => new KeyValuePair<string, Type>(e.Value, e.Key))
            .ToDictionary();
    }
}
