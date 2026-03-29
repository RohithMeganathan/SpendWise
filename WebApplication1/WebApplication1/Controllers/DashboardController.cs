using Microsoft.AspNetCore.Mvc;
using IncomeExpenseManagementApp.DTOs;
using IncomeExpenseManagementApp.Services;

namespace IncomeExpenseManagementApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        /// <summary>
        /// Get dashboard summary with income, expense, and balance
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDTO>> GetDashboardSummary(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var summary = await _dashboardService.GetDashboardSummaryAsync(startDate, endDate);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard summary");
                return StatusCode(500, new { message = "Error retrieving dashboard summary", error = ex.Message });
            }
        }

        /// <summary>
        /// Get monthly summary for the past 12 months
        /// </summary>
        [HttpGet("monthly-summary")]
        public async Task<ActionResult<IEnumerable<MonthlySummaryDTO>>> GetMonthlySummary()
        {
            try
            {
                var summary = await _dashboardService.GetMonthlySummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving monthly summary");
                return StatusCode(500, new { message = "Error retrieving monthly summary", error = ex.Message });
            }
        }
    }
}
