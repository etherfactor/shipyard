namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers;

internal class CarriersControllerTests
{
    private HttpClient _client;

    [SetUp]
    public void SetUp()
    {
        _client = Setup.Client;
    }

    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
    }

    [Test]
    public async Task Test()
    {
        //Fails due to needing to enable UUID on the PGSQL DB; run this during setup

        await _client.GetAsync("api/v1/carriers");
    }
}
