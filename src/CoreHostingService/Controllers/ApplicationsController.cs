using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CoreHostingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IHostingManager _hostingManager;
    private readonly IHealthMonitor _healthMonitor;
    private readonly ILogger<ApplicationsController> _logger;

    public ApplicationsController(
        IHostingManager hostingManager,
        IHealthMonitor healthMonitor,
        ILogger<ApplicationsController> logger)
    {
        _hostingManager = hostingManager;
        _healthMonitor = healthMonitor;
        _logger = logger;
    }

    /// <summary>
    /// Get all hosted applications
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HostedApplication>>> GetApplications()
    {
        try
        {
            var applications = await _hostingManager.GetApplicationsAsync();
            return Ok(applications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving applications");
            return StatusCode(500, new { error = "Failed to retrieve applications" });
        }
    }

    /// <summary>
    /// Get specific application details
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<HostedApplication>> GetApplication(string id)
    {
        try
        {
            var application = await _hostingManager.GetApplicationAsync(id);
            if (application == null)
                return NotFound(new { error = "Application not found" });

            return Ok(application);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving application {ApplicationId}", id);
            return StatusCode(500, new { error = "Failed to retrieve application" });
        }
    }

    /// <summary>
    /// Start an application
    /// </summary>
    [HttpPost("{id}/start")]
    public async Task<ActionResult> StartApplication(string id)
    {
        try
        {
            var application = await _hostingManager.GetApplicationAsync(id);
            if (application == null)
                return NotFound(new { error = "Application not found" });

            await _hostingManager.StartApplicationAsync(id);
            _logger.LogInformation("Application {ApplicationId} started", id);
            return Ok(new { message = "Application started successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting application {ApplicationId}", id);
            return StatusCode(500, new { error = "Failed to start application" });
        }
    }

    /// <summary>
    /// Stop an application
    /// </summary>
    [HttpPost("{id}/stop")]
    public async Task<ActionResult> StopApplication(string id)
    {
        try
        {
            var application = await _hostingManager.GetApplicationAsync(id);
            if (application == null)
                return NotFound(new { error = "Application not found" });

            await _hostingManager.StopApplicationAsync(id);
            _logger.LogInformation("Application {ApplicationId} stopped", id);
            return Ok(new { message = "Application stopped successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping application {ApplicationId}", id);
            return StatusCode(500, new { error = "Failed to stop application" });
        }
    }

    /// <summary>
    /// Restart an application
    /// </summary>
    [HttpPost("{id}/restart")]
    public async Task<ActionResult> RestartApplication(string id)
    {
        try
        {
            var application = await _hostingManager.GetApplicationAsync(id);
            if (application == null)
                return NotFound(new { error = "Application not found" });

            await _hostingManager.RestartApplicationAsync(id);
            _logger.LogInformation("Application {ApplicationId} restarted", id);
            return Ok(new { message = "Application restarted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restarting application {ApplicationId}", id);
            return StatusCode(500, new { error = "Failed to restart application" });
        }
    }

    /// <summary>
    /// Get application health status
    /// </summary>
    [HttpGet("{id}/health")]
    public async Task<ActionResult<HealthStatus>> GetApplicationHealth(string id)
    {
        try
        {
            var application = await _hostingManager.GetApplicationAsync(id);
            if (application == null)
                return NotFound(new { error = "Application not found" });

            var health = await _healthMonitor.GetApplicationHealthAsync(id);
            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving health for application {ApplicationId}", id);
            return StatusCode(500, new { error = "Failed to retrieve application health" });
        }
    }
}
