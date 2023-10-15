using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using SocialNetwork.Data;
using SocialNetwork.Models;
using SocialNetwork.NovaPasta.Accounts;
using SocialNetwork.Services;
using SocialNetwork.ViewModels;
using SocialNetwork.ViewModels.Accounts;

namespace SocialNetwork.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    [HttpPost("v1/accounts/")]
    public async Task<IActionResult> Post(
        [FromBody] RegisterViewModel model,
        [FromServices] AppDbContext context,
        [FromServices] EmailService emailService)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<List<string>>(ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(x => x.ErrorMessage)
                .ToList()
            ));

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Role = model.Role
        };

        user.Password = PasswordHasher.Hash(model.Password);

        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            emailService.Send(user.Name, user.Email, "Bem vindo ao blog!", $"Sua senha é {model.Password}");
            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email,
                model.Password
            }));
        }
        catch (DbUpdateException)
        {
            return StatusCode(400, new ResultViewModel<string>("05X99 - Este E-mail já está cadastrado"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
        }
    }

    [HttpPost("v1/accounts/login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginViewModel model,
        [FromServices] AppDbContext context,
        [FromServices] TokenService tokenService)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<List<string>>(ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(x => x.ErrorMessage)
                .ToList()));

        var user = await context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == model.Email);

        if (user == null)
            return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

        if (!PasswordHasher.Verify(user.Password, model.Password))
            return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

        try
        {
            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token, null));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
        }
    }
}
