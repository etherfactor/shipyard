namespace EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;

public record AspectCase(string Name, Func<Task> Test)
{
    public async Task TestAsync()
    {
        await Test();
    }
}
