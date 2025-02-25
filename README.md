# FastModule

FastModule is a lightweight, extensible, and modular package designed to simplify the development of modular applications in .NET. It provides a foundation for building applications with independent, self-contained modules, promoting reusability, scalability, and maintainability.

## Getting Started

To see FastModule in action, check out the `FastModule.Host.Api` project in this repository. The samples demonstrate elegant extensions around common ASP.NET Core types.

### Installation

Install FastModule via NuGet:

```sh
 dotnet add package FastModule.Core
```

## Features

- **Minimal API Support:** Uses `IEndpointRouteBuilder` for defining routes.
- **Automatic Module Discovery:** FastModule scans and registers implementations automatically.
- **Dependency Injection:** Built-in support for DI.
- **Database Configuration:** Easily configure database connections.

### Routing

FastModule utilizes `IEndpointRouteBuilder` and supports all `IEndpointConventionBuilder` extensions (Minimal APIs). For example, defining a route with authorization:


## Usage


```csharp
app.MapGet("/", () => "Hello World!");
```

### Creating a Module

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastModule();

var app = builder.Build();
app.MapFastModules();
app.Run();

// UserModule.cs
public class UserModule : FastModule
{
    public override IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/api").WithTags("users");
        users.MapGet("/users", async (UserDbContext dbContext) =>
        {
            return await dbContext.Users.ToListAsync();
        });
        return app;
    }
}
```

Run the application:

```sh
dotnet run
```

### Configuration

FastModule automatically scans for implementations and registers them for DI. However, for more control, you can manually register services in the `Register` method and configure database connections in `AddFastModule`:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastModule(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                      x => x.MigrationsHistoryTable("ef_migrations"));
});
```

## Join the Community

Have questions or need help? Join our Discord channel to connect with other developers and get support!

[![Join us on Discord](https://img.shields.io/discord/1343253554382639227?label=Join%20Our%20Discord&logo=discord&style=flat)](https://discord.gg/RYmZrv8a)

---

FastModule is designed to make modular development in .NET simpler and more efficient. Contributions and feedback are welcome!

