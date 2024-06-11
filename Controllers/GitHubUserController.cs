using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class GitHubUserController : ControllerBase
{
    private readonly DatabaseContext _context;

    public GitHubUserController(DatabaseContext context)
    {
        _context = context;
    }

    [HttpGet("/User/{UserId}")]
    public async Task<IActionResult> GetUser(uint UserId)
    {
        var user = await _context.GitHubUsers.FirstOrDefaultAsync(user => user.Id == UserId);

        if (user == null) return NotFound(new { Message = "User not found" });

        return Ok(new { user.Id, user.IsBanned, user.ReportScore });
    }

    [HttpPost("/User/Ban/{UserId}")]
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
    
    [HttpDelete("/User/Ban/{UserId}")]
    public async Task<IActionResult> UnbanUser(uint UserId)
    {
        var user = await _context.GitHubUsers.FirstOrDefaultAsync(user => user.Id == UserId);
        
        if (user == null) return NotFound(new { Message = "User not found" });
        
        user.IsBanned = false;
        await _context.SaveChangesAsync();
        
        return Ok(new { Message = "User has been banned." });
    }
}