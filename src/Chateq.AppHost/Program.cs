var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Chateq_API>("chateq-api");
builder.AddProject<Projects.Chateq_MessageBroker>("chateq-messagebroker");

builder.Build().Run();