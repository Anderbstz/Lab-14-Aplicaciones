using Lab_11_Ander_Bustamante.Application.UseCases.Roles.Commands;
using Lab_11_Ander_Bustamante.Application.UseCases.Roles.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lab_11_Ander_Bustamante.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await sender.Send(new GetRolesQuery()));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand request)
    {
        var role = await sender.Send(request);
        return CreatedAtAction(nameof(GetAll), null, role);
    }
}
