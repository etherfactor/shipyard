var builder = DistributedApplication.CreateBuilder(args);

var rmq = builder.AddRabbitMQ("rmq");

var api = builder.AddProject<Projects.EtherGizmos_Shipyard>("api");
api.WaitFor(rmq).WithReference(rmq, connectionName: "RabbitMQ:ConnectionString");

builder.Build().Run();
