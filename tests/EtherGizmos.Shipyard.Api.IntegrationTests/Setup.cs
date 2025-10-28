using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace EtherGizmos.Shipyard.Api.IntegrationTests;

[SetUpFixture]
internal static class Setup
{
    private static WebApplicationFactory<Program> _waf;

    private static string _pgsqlCstr;
    private static string _rmqCstr;

    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        try
        {
            var pgsql = new PostgreSqlBuilder()
                .WithImage("postgres:17")
                .WithResourceMapping(
                    Encoding.UTF8.GetBytes("""
                        create extension if not exists "uuid-ossp";
                        """),
                    "docker-entrypoint-initdb.d/init.sql")
                .Build();

            await pgsql.StartAsync();

            _pgsqlCstr = pgsql.GetConnectionString();

            var rmq = new RabbitMqBuilder()
                .WithImage("rabbitmq:4")
                .Build();

            await rmq.StartAsync();

            _rmqCstr = rmq.GetConnectionString();
        }
        catch (Exception ex)
        {
            Assert.Ignore(ex.Message);
        }

        var preSettings = new Dictionary<string, string?>()
        {
            ["Artifacts:BasePath"] = "artifacts",
            ["Artifacts:Database:ConnectionId"] = "TestDb",
            ["Connections:TestDb:Type"] = "Database",
            ["Connections:TestDb:PostgreSql:ConnectionString"] = _pgsqlCstr,
            ["Database:ConnectionId"] = "TestDb",
            ["RabbitMq:ConnectionString"] = _rmqCstr,
            ["Security:Certificates:AuthSigning:Type"] = "Certificate",
            ["Security:Certificates:AuthSigning:Text:PublicKey"] = Certificates.TokenSigningPublicKey,
            ["Security:Certificates:AuthSigning:Text:PrivateKey"] = Certificates.TokenSigningPrivateKey,
            ["Security:Certificates:AuthEncryption:Type"] = "Certificate",
            ["Security:Certificates:AuthEncryption:Text:PublicKey"] = Certificates.TokenEncryptionPublicKey,
            ["Security:Certificates:AuthEncryption:Text:PrivateKey"] = Certificates.TokenEncryptionPrivateKey,
            ["Security:OAuth2:SigningCertificate:CertificateId"] = "AuthSigning",
            ["Security:OAuth2:EncryptionCertificate:CertificateId"] = "AuthEncryption",
        };

        _waf = new ConfiguredWebApplicationFactory<Program>(preSettings)
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Integration");
            });
    }

    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        await _waf.DisposeAsync();
    }

    public static HttpClient Client =>
        _waf.CreateClient(new() { HandleCookies = true });
}
