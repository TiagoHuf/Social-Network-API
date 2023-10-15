using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Models;
using SocialNetwork.ViewModels;

namespace SocialNetwork.Controllers;

[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    [HttpGet("v1/users")]
    public async Task<IActionResult> GetAsync(
            [FromServices] AppDbContext context)
    {
        try
        {
            var categories = await context.Users.ToListAsync();
            return Ok(new ResultViewModel<List<User>>(categories));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<List<User>>("05X04 - Falha interna no servidor"));
        }
    }
}
