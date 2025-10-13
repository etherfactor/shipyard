using System.Text;
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
