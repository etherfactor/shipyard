var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");

var database = postgres.AddDatabase("postgres-db", databaseName: "shipyard");

var rmq = builder.AddRabbitMQ("rmq");

var selenium = builder.AddContainer("selenium", "selenium/standalone-chromium:137.0");
selenium.WithHttpEndpoint(targetPort: 4444, name: "endpoint");

var api = builder.AddProject<Projects.EtherGizmos_Shipyard_Api>("api");
api.WaitFor(database).WithReference(database, connectionName: "PostgreSql:ConnectionString");

var worker = builder.AddProject<Projects.EtherGizmos_Shipyard_Worker>("worker");
worker.WaitFor(database).WithReference(database, connectionName: "PostgreSql:ConnectionString");
worker.WaitFor(rmq).WithReference(rmq, connectionName: "RabbitMq:ConnectionString");
worker.WaitFor(selenium).WithEnvironment("Selenium:ConnectionString", () => selenium.GetEndpoint("endpoint").Url + "/wd/hub");

builder.Build().Run();
