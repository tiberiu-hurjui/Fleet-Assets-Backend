using Fleet_Assets_Backend.Api.Middleware;
using Fleet_Assets_Backend.Application.Interfaces;
using Fleet_Assets_Backend.Application.Services;
using Fleet_Assets_Backend.Infrastructure.Interfaces;
using Fleet_Assets_Backend.Infrastructure.Persistence;
using Fleet_Assets_Backend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FleetAssetsDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")));

// Repositories (Infrastructure)
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IEventLogRepository, EventLogRepository>();

// Services (Application)
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IEventQueryService, EventQueryService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.Use(async (context, next) =>
{
    const string header = "X-Correlation-Id";

    var correlationId =
        context.Request.Headers.TryGetValue(header, out var cid) && cid.Count > 0
            ? cid.ToString()
            : Guid.NewGuid().ToString();

    context.Response.Headers[header] = correlationId;

    using (context.RequestServices
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("Correlation")
        .BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
    {
        await next();
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
