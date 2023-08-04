using Application.Activities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Core;
using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope(); // Create a scope for the lifetime of the request
var services = scope.ServiceProvider; // Get access to the services we've registered
try
{
  var context = services.GetRequiredService<DataContext>(); // Get access to the DataContext
  await context.Database.MigrateAsync(); // Migrate the database
  await Seed.SeedData(context); // Seed the database
}
catch (Exception ex)
{
  var logger = services.GetRequiredService<ILogger<Program>>();
  logger.LogError(ex, "An error occurred during migration");
}

app.Run();
