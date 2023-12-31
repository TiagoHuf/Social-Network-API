﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using SocialNetwork.Data;
using SocialNetwork.Models;
using SocialNetwork.NovaPasta.Accounts;
using SocialNetwork.Services;
using SocialNetwork.ViewModels;
using SocialNetwork.ViewModels.Accounts;
using System.Reflection;
using System.Security.Claims;

namespace SocialNetwork.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    [HttpPost("v1/accounts/")]
    public async Task<IActionResult> Post(
        [FromBody] RegisterViewModel model,
        [FromServices] AppDbContext context,
        [FromServices] EmailService emailService,
        [FromServices] TokenService tokenService)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<List<string>>(ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(x => x.ErrorMessage)
                .ToList()
            ));

        var newUser = new User
        {
            Name = model.Name,
            Email = model.Email,
            Role = model.Role,
            Password = PasswordHasher.Hash(model.Password)
        };

        try
        {
            await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();

            string pathToHtmlFile = "Resources/WelcomeEmailTemplate.html";
            string htmlContent = System.IO.File.ReadAllText(pathToHtmlFile);

            var user = context.Users.FirstOrDefault(u => u.Email == newUser.Email);

            var token = tokenService.GenerateAuthenticationToken(user.Id);

            htmlContent = htmlContent.Replace("{link}", token);

            emailService.Send(user.Name, user.Email, "Bem vindo ao blog!", htmlContent);
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

        if (user.Authenticated is false)
            return StatusCode(403, new ResultViewModel<string>("Email não confirmado"));

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

    [HttpPost("v1/accounts/authentication/{token}")]
    public async Task<IActionResult> Authenticate(
        [FromRoute] string token,
        [FromServices] AppDbContext context,
        [FromServices] TokenService tokenService)
    {
        var userId = tokenService.GetUserIdFromToken(token);

        var user = await context.Users.FindAsync(userId);

        if (user != null)
        {
            user.Authenticated = true;

            context.Entry(user).State = EntityState.Modified;

            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<string>("Email confirmado com sucesso"));
        }
        else
        {
            return NotFound(new { message = "Usuário não encontrado" });
        }
    }
}
