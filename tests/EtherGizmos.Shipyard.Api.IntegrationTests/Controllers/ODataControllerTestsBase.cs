using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;
using System.Collections.Concurrent;
using System.Net.Http.Json;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers;

internal abstract class ODataControllerTestsBase<TEntity, TId>
    where TEntity : class, new()
{
    protected abstract IODataResourceSpec<TEntity, TId> Specification { get; }

    protected virtual IReadOnlyCollection<IAspect<TEntity, TId>> Aspects { get; } =
    [
        new SearchSelectOptionAspect<TEntity, TId>(),
        new GetSelectOptionAspect<TEntity, TId>(),
        new CreateSelectOptionAspect<TEntity, TId>(),
        new PatchSelectOptionAspect<TEntity, TId>(),
    ];

    protected HttpClient Client { get; private set; }

    [SetUp]
    public void SetUpClient()
    {
        Client = Setup.Client;
    }

    [TearDown]
    public void TearDownClient()
    {
        Client?.Dispose();
    }

    protected async Task<TEntity> CreateAsync(
        TEntity? entity = null,
        CancellationToken cancellationToken = default)
    {
        var content = entity is not null ? JsonContent.Create(entity) : Specification.Create();

        var response = await Client.PostAsync(Specification.BaseRoute, content, cancellationToken);
        var created = await response.Content.ReadFromJsonAsync<TEntity>(cancellationToken: cancellationToken);

        return created!;
    }

    [Test]
    public async Task Tests()
    {
        var exceptions = new ConcurrentBag<Exception>();

        foreach (var aspect in Aspects)
        {
            var tests = aspect
                .Build(Specification, new FixtureContext(Client))
                .ToList();

            foreach (var test in tests)
            {
                try
                {
                    await test.TestAsync();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
        }

        if (!exceptions.IsEmpty)
        {
            throw new AggregateException(exceptions);
        }
    }
}
