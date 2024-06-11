using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportsController : ControllerBase
{
    private readonly DatabaseContext _context;

    public ReportsController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpGet("All/{UserId}")]
    public async Task<IActionResult> GetAllUserReports(uint UserId)
    {
        var reports = await _context.Reports.Where(report => report.GitHubUserId == UserId).Select(report => new {
            report.Id,
            report.GitHubUserId,
            report.ReportReason,
            report.Status
        }).ToListAsync();

        if (reports.Count == 0)
            return NotFound(new { Message = "No reports found for this user" });
        
        return Ok(reports);
    }

    [HttpPost("{UserId}")]
    public async Task<IActionResult> FileUserReport(uint UserId, [FromBody] ReportRequest request)
    {
        var user = await _context.GitHubUsers.FirstOrDefaultAsync(user => user.Id == UserId);

        if (user == null)
        {
            user = new GitHubUser()
            {
                Id = UserId,
                IsBanned = false,
                ReportScore = 1
            };

            _context.GitHubUsers.Add(user);

            await _context.SaveChangesAsync();
        }

        var report = new Report()
        {
            ReportReason = request.ReportReason,
            GitHubUserId = UserId,
            GitHubUser = user
        };
        
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Report filed successfully" });
    }

    [HttpPut("{ReportId}")]
    public async Task<IActionResult> AcceptReport(uint ReportId)
    {
        var report = await _context.Reports.Include(report => report.GitHubUser).FirstOrDefaultAsync(report => report.Id == ReportId);

        if (report == null)
            return NotFound(new { Message = "Report not found" });
        if (report.Status != ReportStatus.Pending)
            return BadRequest(new { Message = "Report has already been processed" });

        report.GitHubUser.IsBanned = true;
        report.GitHubUser.ReportScore = 0;
        report.Status = ReportStatus.Accepted;
        
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Report has been accepted" });
    }
    
    [HttpDelete("{ReportId}")]
    public async Task<IActionResult> RejectReport(uint ReportId)
    {
        var report = await _context.Reports.Include(report => report.GitHubUser).FirstOrDefaultAsync(report => report.Id == ReportId);
        if (report == null)
            return NotFound(new { Message = "Report not found" });
        if (report.Status != ReportStatus.Pending)
            return BadRequest(new { Message = "Report has already been processed" });

        report.GitHubUser.ReportScore = 0;
        report.Status = ReportStatus.Rejected;
        
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Report has been rejected" });
    }
}