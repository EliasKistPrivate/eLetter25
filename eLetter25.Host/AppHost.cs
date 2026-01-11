using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var usersDb = builder.AddPostgres("Identity")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("users-db");

var persistence = builder.AddSqlServer("Persistence")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var db = persistence.AddDatabase("eletter25-db");

var letterApi = builder.AddProject<eLetter25_API>("API")
    .WithReference(usersDb)
    .WithReference(db)
    .WaitFor(db)
    .WaitFor(usersDb);

var client = builder.AddNpmApp("Client", Path.Combine("..", "eLetter25.Client", "eLetter25.Client"), "start")
    .WithReference(letterApi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();