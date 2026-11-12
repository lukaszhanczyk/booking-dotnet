var builder = WebApplication.CreateBuilder(args);

const string connectionStringName = "Default";
var connectionString = builder.Configuration.GetConnectionString(connectionStringName) 
    ?? throw new InvalidOperationException(
        $"Connection string '{connectionStringName}' not found. " +
        $"Set ConnectionStrings:{connectionStringName} (user-secrets or env ConnectionStrings__{connectionStringName}).");

var app = builder.Build();


app.MapGet("/", () => "Hello World!");
app.MapGet("/api/health", async context => Results.Ok(new { status = "ok", time = DateTimeOffset.UtcNow }));

app.Run();