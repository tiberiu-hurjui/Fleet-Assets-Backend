using Fleet_Assets_Backend.Application.Events;
using Fleet_Assets_Backend.Application.Interfaces;
using Fleet_Assets_Backend.Application.Services;
using Fleet_Assets_Backend.Infrasturcture.Interfaces;
using Fleet_Assets_Backend.Infrasturcture.Persistence;
using Fleet_Assets_Backend.Infrasturcture.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    const string header = "X-Correlation-Id";
    if (!context.Request.Headers.TryGetValue(header, out var cid) || cid.Count == 0)
    {
        context.Response.Headers[header] = Guid.NewGuid().ToString();
    }
    else
    {
        context.Response.Headers[header] = cid.ToString();
    }

    await next();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
