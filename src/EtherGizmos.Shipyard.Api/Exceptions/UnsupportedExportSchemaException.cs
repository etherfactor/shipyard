namespace EtherGizmos.Shipyard.Exceptions;

public class UnsupportedExportSchemaException : Exception
{
    public string Kind { get; }

    public int SchemaVersion { get; }

    public int MaxSchemaVersion { get; }

    public UnsupportedExportSchemaException(
        string kind,
        int schemaVersion,
        int maxVersion,
        string message)
        : this(kind, schemaVersion, maxVersion, message, null) { }

    public UnsupportedExportSchemaException(
        string kind,
        int schemaVersion,
        int maxVersion,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Kind = kind;
        SchemaVersion = schemaVersion;
        MaxSchemaVersion = maxVersion;
    }
}
