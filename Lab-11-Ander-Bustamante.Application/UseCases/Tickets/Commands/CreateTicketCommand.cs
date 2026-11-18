using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Domain.Models;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Tickets.Commands;

public record CreateTicketCommand(Guid UserId, string Title, string? Description) : IRequest<TicketDto>;

internal sealed class CreateTicketCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateTicketCommand, TicketDto>
{
    public async Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new InvalidOperationException("El titulo es obligatorio.");
        }

        var user = await unitOfWork.Users.FirstOrDefaultAsync(item => item.UserId == request.UserId);
        if (user is null)
        {
            throw new InvalidOperationException("El usuario indicado no existe.");
        }

        var ticket = new Ticket
        {
            TicketId = Guid.NewGuid(),
            UserId = request.UserId,
            Title = request.Title,
            Description = request.Description,
            Status = "abierto",
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.Tickets.AddAsync(ticket);
        await unitOfWork.CompleteAsync();

        return new TicketDto
        {
            TicketId = ticket.TicketId,
            UserId = ticket.UserId,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            ClosedAt = ticket.ClosedAt
        };
    }
}
