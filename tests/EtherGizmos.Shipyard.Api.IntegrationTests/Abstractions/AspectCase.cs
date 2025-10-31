namespace EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;

public record AspectCase(string Name, Func<FixtureContext, Task> Test)
{
    public async Task TestAsync(
        FixtureContext context)
    {
        await Test(context);
    }

    public override string ToString() => Name;
}
