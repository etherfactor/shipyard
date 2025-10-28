var builder = DistributedApplication.CreateBuilder(args);

var postgresUsername = builder.AddParameter("postgres-username", "postgres", secret: true);
var postgresPassword = builder.AddParameter("postgres-password", "nQW~xg-Z2Tzr5pkj*edJ5f", secret: true);

var postgres = builder.AddPostgres("postgres", port: 57691, userName: postgresUsername, password: postgresPassword)
    .WithDataVolume(isReadOnly: false);

var database = postgres.AddDatabase("postgres-db", databaseName: "shipyard");

var rabbitmq = builder.AddRabbitMQ("rabbitmq");
rabbitmq.WithManagementPlugin();

var selenium = builder.AddContainer("selenium", "selenium/standalone-chromium:137.0");
selenium.WithHttpEndpoint(targetPort: 4444, name: "endpoint");

var api = builder.AddProject<Projects.EtherGizmos_Shipyard_Api>("api");
api.WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");
api.WaitFor(database).WithReference(database, connectionName: "Connections:AspireDb:PostgreSql:ConnectionString");
api.WaitFor(rabbitmq).WithReference(rabbitmq, connectionName: "RabbitMq:ConnectionString");
api.WithEnvironment("Connections:AspireDb:Type", "Database");
api.WithEnvironment("Database:ConnectionId", "AspireDb");

var worker = builder.AddProject<Projects.EtherGizmos_Shipyard_Worker>("worker");
worker.WithEnvironment("DOTNET_ENVIRONMENT", "Development");
worker.WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");
worker.WaitFor(database).WithReference(database, connectionName: "Connections:AspireDb:PostgreSql:ConnectionString");
worker.WaitFor(rabbitmq).WithReference(rabbitmq, connectionName: "RabbitMq:ConnectionString");
worker.WaitFor(selenium).WithEnvironment("Selenium:ConnectionString", () => selenium.GetEndpoint("endpoint").Url + "/wd/hub");
worker.WithEnvironment("Connections:AspireDb:Type", "Database");
worker.WithEnvironment("Database:ConnectionId", "AspireDb");

builder.Build().Run();
