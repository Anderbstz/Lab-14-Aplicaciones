using Lab_11_Ander_Bustamante.Application.UseCases.Responses.Commands;
using Lab_11_Ander_Bustamante.Application.UseCases.Responses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab_11_Ander_Bustamante.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResponsesController(ISender sender) : ControllerBase
{
    [HttpGet("ticket/{ticketId:guid}")]
    public async Task<IActionResult> GetByTicket(Guid ticketId)
    {
        return Ok(await sender.Send(new GetResponsesByTicketQuery(ticketId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateResponseCommand request)
    {
        var response = await sender.Send(request);
        return CreatedAtAction(nameof(GetByTicket), new { ticketId = response.TicketId }, response);
    }
}
