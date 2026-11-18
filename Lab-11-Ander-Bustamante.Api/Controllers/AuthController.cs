using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab_11_Ander_Bustamante.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        return Ok(await authService.LoginAsync(request.Username, request.Password));
    }
}
