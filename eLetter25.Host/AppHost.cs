using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<eLetter25_API>("letterAPI");

builder.Build().Run();