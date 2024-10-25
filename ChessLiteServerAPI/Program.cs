using Microsoft.Extensions.Hosting;
using ChessLiteServerAPI.Data;
using ChessLiteServerAPI.Models;
using Microsoft.EntityFrameworkCore;
using ChessLiteServerAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ChessLiteServerAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChessLiteServerAPIContext")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new ChessPieceArrayConverter());
        options.JsonSerializerOptions.Converters.Add(new TupleConverter());
    });

// Use Scoped for GameRecordService:
// GameRecordService is request-specific and may interact with a DbContext or other per-request state.
// Each HTTP request gets a new instance to avoid conflicts and ensure thread safety.
// This ensures that GameRecordService has the same lifetime as the request it serves.
builder.Services.AddScoped<GameRecordService>();

// Use Singleton for TaskManager:
// TaskManager is responsible for managing and tracking background tasks across multiple requests.
// It needs to maintain a consistent state throughout the application’s lifetime.
// A singleton ensures that the same instance is shared across all requests, 
// allowing centralized tracking and management of long-running tasks.
builder.Services.AddSingleton<TaskManager>();


var app = builder.Build();

// Hook into the shutdown lifecycle events
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
var taskManager = app.Services.GetRequiredService<TaskManager>();

lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("Application stopping... Waiting for background tasks to complete.");

    // Wait for all background tasks to complete before shutting down
    taskManager.WaitForAllTasks().Wait();  // Synchronous wait
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
