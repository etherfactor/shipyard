using EtherGizmos.Shipyard.Abstractions;
using EtherGizmos.Shipyard.Controllers.Specifications;
using System.Text;

namespace EtherGizmos.Shipyard.Controllers;

internal class ImportExportControllerTests : IntegrationTestBase
{
    [Test]
    public async Task ExportCarrier_WithValidIdAcceptYaml_ShouldSucceed()
    {
        //Arrange
        var (carrier, id) = await CarriersControllerV1Spec.Instance.Records.AcquireAsync(FixtureContext.Instance, AcquirePurpose.ForRead);
        using var client = FixtureContext.Instance.GetClientWithCapabilities(Setup.OwnerUserId.ToString(), capabilities: "Carrier:7");
        client.DefaultRequestHeaders.Accept.Add(new("application/yaml"));

        //Act
        using var response = await client.GetAsync($"/api/v1/carriers({id})/export");

        //Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/yaml"));
        }
    }

    [Test]
    public async Task ExportCarrier_WithValidIdAcceptJson_ShouldSucceed()
    {
        //Arrange
        var (carrier, id) = await CarriersControllerV1Spec.Instance.Records.AcquireAsync(FixtureContext.Instance, AcquirePurpose.ForRead);
        using var client = FixtureContext.Instance.GetClientWithCapabilities(Setup.OwnerUserId.ToString(), capabilities: "Carrier:7");
        client.DefaultRequestHeaders.Accept.Add(new("application/json"));

        //Act
        using var response = await client.GetAsync($"/api/v1/carriers({id})/export");

        //Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
        }
    }

    [Test]
    public async Task Import_WithValidCarrierContentTypeYaml_ShouldSucceed()
    {
        //Arrange
        var (carrier, id) = await CarriersControllerV1Spec.Instance.Records.AcquireAsync(FixtureContext.Instance, AcquirePurpose.ForUpdate);
        using var client = FixtureContext.Instance.GetClientWithCapabilities(Setup.OwnerUserId.ToString(), capabilities: "Carrier:7");
        client.DefaultRequestHeaders.Accept.Add(new("application/yaml"));

        using var exportResponse = await client.GetAsync($"/api/v1/carriers({id})/export");
        var content = await exportResponse.Content.ReadAsStringAsync();

        //Act
        using var response = await client.PostAsync($"/api/v1/import", new StringContent(content, Encoding.UTF8, "application/yaml"));

        //Assert
        Assert.That(exportResponse.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task Import_WithValidCarrierContentTypeJson_ShouldSucceed()
    {
        //Arrange
        var (carrier, id) = await CarriersControllerV1Spec.Instance.Records.AcquireAsync(FixtureContext.Instance, AcquirePurpose.ForUpdate);
        using var client = FixtureContext.Instance.GetClientWithCapabilities(Setup.OwnerUserId.ToString(), capabilities: "Carrier:7");
        client.DefaultRequestHeaders.Accept.Add(new("application/json"));

        using var exportResponse = await client.GetAsync($"/api/v1/carriers({id})/export");
        var content = await exportResponse.Content.ReadAsStringAsync();

        //Act
        using var response = await client.PostAsync($"/api/v1/import", new StringContent(content, Encoding.UTF8, "application/json"));

        //Assert
        Assert.That(exportResponse.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task VerifyImport_WithValidCarrierContentTypeYaml_ShouldSucceed()
    {
        //Arrange
        var (carrier, id) = await CarriersControllerV1Spec.Instance.Records.AcquireAsync(FixtureContext.Instance, AcquirePurpose.ForUpdate);
        using var client = FixtureContext.Instance.GetClientWithCapabilities(Setup.OwnerUserId.ToString(), capabilities: "Carrier:7");

        using var exportResponse = await client.GetAsync($"/api/v1/carriers({id})/export");
        var content = await exportResponse.Content.ReadAsStringAsync();

        //Act
        using var response = await client.PostAsync($"/api/v1/import/verify", new StringContent(content, Encoding.UTF8, "application/yaml"));

        //Assert
        Assert.That(exportResponse.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task VerifyImport_WithValidCarrierContentTypeJson_ShouldSucceed()
    {
        //Arrange
        var (carrier, id) = await CarriersControllerV1Spec.Instance.Records.AcquireAsync(FixtureContext.Instance, AcquirePurpose.ForUpdate);
        using var client = FixtureContext.Instance.GetClientWithCapabilities(Setup.OwnerUserId.ToString(), capabilities: "Carrier:7");
        client.DefaultRequestHeaders.Accept.Add(new("application/json"));

        using var exportResponse = await client.GetAsync($"/api/v1/carriers({id})/export");
        var content = await exportResponse.Content.ReadAsStringAsync();

        //Act
        using var response = await client.PostAsync($"/api/v1/import/verify", new StringContent(content, Encoding.UTF8, "application/json"));

        //Assert
        Assert.That(exportResponse.IsSuccessStatusCode, Is.True);
    }
}
