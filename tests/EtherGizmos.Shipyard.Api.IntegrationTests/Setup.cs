using Npgsql;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace EtherGizmos.Shipyard.Api.IntegrationTests;

[SetUpFixture]
internal static class Setup
{
    private static ConfiguredWebApplicationFactory<Program> _waf;

    private static string _pgsqlCstr;
    private static string _rmqCstr;

    [OneTimeSetUp]
    public static async Task OneTimeSetUp()
    {
        try
        {
            var pgsql = new PostgreSqlBuilder()
                .WithImage("postgres:17")
                .Build();

            await pgsql.StartAsync();

            _pgsqlCstr = pgsql.GetConnectionString();

            var rmq = new RabbitMqBuilder()
                .WithImage("rabbitmq:4")
                .Build();

            await rmq.StartAsync();

            _rmqCstr = rmq.GetConnectionString();

            using var conn = new NpgsqlConnection(_pgsqlCstr);

            await conn.OpenAsync();

            using var command = conn.CreateCommand();

            command.CommandText = """
                create extension if not exists "uuid-ossp";
                """;

            await command.ExecuteNonQueryAsync();
        }
        catch
        {
            Assert.Ignore();
        }

        _waf = new(new Dictionary<string, string?>()
        {
            ["Connections:TestDb:Type"] = "Database",
            ["Connections:TestDb:PostgreSql:ConnectionString"] = _pgsqlCstr,
            ["Database:ConnectionId"] = "TestDb",
            ["RabbitMq:ConnectionString"] = _rmqCstr,
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
