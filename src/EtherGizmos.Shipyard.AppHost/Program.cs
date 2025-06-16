var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EtherGizmos_Shipyard>("ethergizmos-shipyard");

builder.Build().Run();
