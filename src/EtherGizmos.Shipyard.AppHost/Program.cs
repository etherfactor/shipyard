var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");

var rmq = builder.AddRabbitMQ("rmq");

var api = builder.AddProject<Projects.EtherGizmos_Shipyard>("api");
api.WaitFor(postgres).WithReference(postgres, connectionName: "PostgreSQL:ConnectionString");

var worker = builder.AddProject<Projects.EtherGizmos_Shipyard_Worker>("worker");
worker.WaitFor(postgres).WithReference(postgres, connectionName: "PostgreSQL:ConnectionString");
worker.WaitFor(rmq).WithReference(rmq, connectionName: "RabbitMQ:ConnectionString");

builder.Build().Run();
