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
                ReportScore = 0
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
        
        user.ReportScore++;
        
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Report filed successfully" });
    }

    [HttpPut("{ReportId}")]
    public async Task<IActionResult> AcceptReport(uint ReportId)
    {
        var report = await _context.Reports.Include(r => r.GitHubUser).FirstOrDefaultAsync(r => r.Id == ReportId);

        if (report == null)
            return NotFound(new { Message = "Report not found" });
        if (report.Status != ReportStatus.Pending)
            return BadRequest(new { Message = "Report has already been processed" });

        var userReports = await _context.Reports.Where(r => r.GitHubUserId == report.GitHubUserId && r.Status == ReportStatus.Pending).ToListAsync();

        foreach (var r in userReports)
        {
            if (r.Id == ReportId)
            {
                r.Status = ReportStatus.Accepted;
                report.GitHubUser.IsBanned = true;
            }
            else
            {
                r.Status = ReportStatus.Duplicate;
            }
        }
    
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Report has been accepted and other pending reports removed" });
    }

    
    [HttpDelete("{ReportId}")]
    public async Task<IActionResult> RejectReport(uint ReportId)
    {
        var report = await _context.Reports.Include(r => r.GitHubUser).FirstOrDefaultAsync(r => r.Id == ReportId);
        if (report == null)
            return NotFound(new { Message = "Report not found" });
        if (report.Status != ReportStatus.Pending)
            return BadRequest(new { Message = "Report has already been processed" });

        var userReports = await _context.Reports.Where(r => r.GitHubUserId == report.GitHubUserId && r.Status == ReportStatus.Pending).ToListAsync();

        foreach (var r in userReports)
        {
            if (r.Id == ReportId)
            {
                r.Status = ReportStatus.Rejected;
            }
            else
            {
                r.Status = ReportStatus.Duplicate;
            }
        }

        report.GitHubUser.ReportScore = 0;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Report has been rejected and other pending reports removed" });
    }
}