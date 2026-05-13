using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CoreHostingService;

public class Program
{
    public static void Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/hosting-service-.txt", 
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        try
        {
            Log.Information("Starting ASP.NET Core Hosting Service...");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                services.AddControllers();
                services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
                });
                services.AddSingleton<IHostingManager, HostingManager>();
                services.AddSingleton<IHealthMonitor, HealthMonitor>();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseCors("AllowAll");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/health", async context =>
            {
                var healthMonitor = context.RequestServices.GetRequiredService<IHealthMonitor>();
                var status = await healthMonitor.GetSystemHealthAsync();
                await context.Response.WriteAsJsonAsync(status);
            });
        });
    }
}

public interface IHostingManager
{
    Task<IEnumerable<HostedApplication>> GetApplicationsAsync();
    Task<HostedApplication?> GetApplicationAsync(string id);
    Task StartApplicationAsync(string id);
    Task StopApplicationAsync(string id);
    Task RestartApplicationAsync(string id);
}

public interface IHealthMonitor
{
    Task<HealthStatus> GetSystemHealthAsync();
    Task<HealthStatus> GetApplicationHealthAsync(string applicationId);
}

public class HostingManager : IHostingManager
{
    private readonly ILogger<HostingManager> _logger;
    private readonly Dictionary<string, HostedApplication> _applications;

    public HostingManager(ILogger<HostingManager> logger)
    {
        _logger = logger;
        _applications = new Dictionary<string, HostedApplication>();
        InitializeApplications();
    }

    private void InitializeApplications()
    {
        _logger.LogInformation("Initializing hosted applications");
        // Applications will be loaded from configuration
    }

    public Task<IEnumerable<HostedApplication>> GetApplicationsAsync()
    {
        return Task.FromResult(_applications.Values.AsEnumerable());
    }

    public Task<HostedApplication?> GetApplicationAsync(string id)
    {
        _applications.TryGetValue(id, out var app);
        return Task.FromResult(app);
    }

    public Task StartApplicationAsync(string id)
    {
        _logger.LogInformation("Starting application: {ApplicationId}", id);
        return Task.CompletedTask;
    }

    public Task StopApplicationAsync(string id)
    {
        _logger.LogInformation("Stopping application: {ApplicationId}", id);
        return Task.CompletedTask;
    }

    public Task RestartApplicationAsync(string id)
    {
        _logger.LogInformation("Restarting application: {ApplicationId}", id);
        return Task.CompletedTask;
    }
}

public class HealthMonitor : IHealthMonitor
{
    private readonly ILogger<HealthMonitor> _logger;

    public HealthMonitor(ILogger<HealthMonitor> logger)
    {
        _logger = logger;
    }

    public Task<HealthStatus> GetSystemHealthAsync()
    {
        var status = new HealthStatus
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Uptime = GetSystemUptime(),
            MemoryUsageMB = GC.GetTotalMemory(false) / 1024 / 1024
        };
        return Task.FromResult(status);
    }

    public Task<HealthStatus> GetApplicationHealthAsync(string applicationId)
    {
        var status = new HealthStatus
        {
            Status = "Unknown",
            Timestamp = DateTime.UtcNow
        };
        return Task.FromResult(status);
    }

    private static TimeSpan GetSystemUptime()
    {
        return DateTime.UtcNow - DateTime.UtcNow.AddMilliseconds(-Environment.TickCount);
    }
}

public class HostedApplication
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool AutoRestart { get; set; }
    public string Status { get; set; } = "Stopped";
}

public class HealthStatus
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public TimeSpan? Uptime { get; set; }
    public long? MemoryUsageMB { get; set; }
}
