using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using System.Data;
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
            var pgsql = new PostgreSqlBuilder("postgres:17")
                .WithResourceMapping(
                    Encoding.UTF8.GetBytes("""
                        create extension if not exists "uuid-ossp";
                        """),
                    "docker-entrypoint-initdb.d/init.sql")
                .Build();

            await pgsql.StartAsync();

            _pgsqlCstr = pgsql.GetConnectionString();

            var rmq = new RabbitMqBuilder("rabbitmq:4")
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
            ["Database:ConnectionId"] = "TestDatabase",
            ["Connections:TestDatabase:Type"] = "Database",
            ["Connections:TestDatabase:PostgreSql:ConnectionString"] = _pgsqlCstr,
            ["MessageBroker:ConnectionId"] = "TestMessageBroker",
            ["Connections:TestMessageBroker:Type"] = "MessageBroker",
            ["Connections:TestMessageBroker:RabbitMQ:ConnectionString"] = _rmqCstr,
            ["Keys:AuthSigning:Type"] = "Asymmetric",
            ["Keys:AuthSigning:PfxFile:Path"] = Certificates.TokenSigningPath,
            ["Keys:AuthSigning:PfxFile:AutoGenerate"] = "true",
            ["Keys:AuthEncryption:Type"] = "Asymmetric",
            ["Keys:AuthEncryption:PfxFile:Path"] = Certificates.TokenEncryptionPath,
            ["Keys:AuthEncryption:PfxFile:AutoGenerate"] = "true",
            ["Security:OAuth2:SigningCertificate:KeyId"] = "AuthSigning",
            ["Security:OAuth2:EncryptionCertificate:KeyId"] = "AuthEncryption",
        };

        _waf = new ConfiguredWebApplicationFactory<Program>(preSettings)
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Integration");
            });

        using var client = Client;

        await client.GetAsync("/api/v1/packages");

        using var connection = new NpgsqlConnection(_pgsqlCstr);
        await connection.OpenAsync();

        using var command = connection.CreateCommand();

        command.CommandText = """
            select user_id
              from users
              where username = 'admin';
            """;

        using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        OwnerUserId = reader.GetGuid("user_id");
    }

    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        await _waf.DisposeAsync();
    }

    public static Guid OwnerUserId { get; private set; }

    public static HttpClient Client
        => _waf.CreateClient(new() { HandleCookies = true });

    public static IServiceProvider Services
        => _waf.Services;
}
