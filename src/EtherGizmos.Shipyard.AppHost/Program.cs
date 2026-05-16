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

var mailpit = builder.AddMailPit("mailpit");

var api = builder.AddProject<Projects.EtherGizmos_Shipyard_Api>("api");
api.WithHttpHealthCheck(path: "/health");
api.WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");
api.WithEnvironment("WebUI:BaseUrl", "https://localhost:53906");

api.WithEnvironment("Database:ConnectionId", "AspireDb");
api.WithEnvironment("Connections:AspireDb:Type", "Database");
api.WaitFor(database).WithReference(database, connectionName: "Connections:AspireDb:PostgreSql:ConnectionString");

api.WithEnvironment("MessageBroker:ConnectionId", "AspireBus");
api.WithEnvironment("Connections:AspireBus:Type", "MessageBroker");
api.WaitFor(rabbitmq).WithReference(rabbitmq, connectionName: "Connections:AspireBus:RabbitMQ:ConnectionString");

api.WithEnvironment("Email:ConnectionId", "AspireEmail");
api.WithEnvironment("Connections:AspireEmail:Type", "Email");
api.WaitFor(mailpit).WithEnvironment("Connections:AspireEmail:Smtp:Host", () => mailpit.GetEndpoint("smtp").Host)
    .WithEnvironment("Connections:AspireEmail:Smtp:Port", () => mailpit.GetEndpoint("smtp").Port.ToString())
    .WithEnvironment("Connections:AspireEmail:Smtp:UseSsl", "false");

var worker = builder.AddProject<Projects.EtherGizmos_Shipyard_Worker>("worker");
worker.WithEnvironment("DOTNET_ENVIRONMENT", "Development");
worker.WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

worker.WaitFor(api);

worker.WithEnvironment("MessageBroker:ConnectionId", "AspireBus");
worker.WithEnvironment("Connections:AspireBus:Type", "MessageBroker");
worker.WaitFor(rabbitmq).WithReference(rabbitmq, connectionName: "Connections:AspireBus:RabbitMQ:ConnectionString");

worker.WaitFor(selenium).WithEnvironment("Selenium:ConnectionString", () => selenium.GetEndpoint("endpoint").Url + "/wd/hub");

builder.Build().Run();
