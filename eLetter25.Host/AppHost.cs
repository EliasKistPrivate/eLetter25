using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloak("keycloak", 8080)
    .WithDataVolume()
    .WithOtlpExporter();

var sql = builder.AddSqlServer("sqlserver")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("eletter25db");

builder.AddProject<eLetter25_API>("letterAPI")
    .WithReference(keycloak)
    .WithReference(db)
    .WaitFor(keycloak)
    .WaitFor(db);

builder.Build().Run();