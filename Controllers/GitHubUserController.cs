using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("/Api/GitHub/User")]
public class GitHubUserController : ControllerBase
{
    private readonly DatabaseContext _context;

    public GitHubUserController(DatabaseContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet("{UserId}")]
    public async Task<IActionResult> GetUser(uint UserId)
    {
        var user = await _context.GitHubUsers.FirstOrDefaultAsync(user => user.Id == UserId);

        if (user == null) return NotFound(new { Message = "User not found" });

        return Ok(new { user.Id, user.IsBanned, user.ReportScore });
    }

    [Authorize]
    [HttpPost("Ban/{UserId}")]
    public async Task<IActionResult> BanUser(uint UserId)
    {
        var user = await _context.GitHubUsers.FirstOrDefaultAsync(user => user.Id == UserId);

        if (user == null)
        {
            user = new GitHubUser { Id = UserId, IsBanned = true };
            _context.GitHubUsers.Add(user);
        }
        else
        {
            user.IsBanned = true;
        }

        await _context.SaveChangesAsync();
        
        return Ok(new { Message = "User has been banned." });
    }
    
    [Authorize]
    [HttpDelete("Ban/{UserId}")]
    public async Task<IActionResult> UnbanUser(uint UserId)
    {
        var user = await _context.GitHubUsers.FirstOrDefaultAsync(user => user.Id == UserId);
        
        if (user == null) return NotFound(new { Message = "User not found" });
        
        user.IsBanned = false;
        user.ReportScore = 0;
        await _context.SaveChangesAsync();
        
        return Ok(new { Message = "User has been unbanned." });
    }
}