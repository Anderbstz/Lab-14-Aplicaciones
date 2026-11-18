using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Tickets.Commands;

public record ChangeTicketStatusCommand(Guid TicketId, string Status) : IRequest<TicketDto>;

internal sealed class ChangeTicketStatusCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeTicketStatusCommand, TicketDto>
{
    public async Task<TicketDto> Handle(ChangeTicketStatusCommand request, CancellationToken cancellationToken)
    {
        if (request.Status != "abierto" && request.Status != "en_proceso" && request.Status != "cerrado")
        {
            throw new InvalidOperationException("El estado debe ser abierto, en_proceso o cerrado.");
        }

        var ticket = await unitOfWork.Tickets.FirstOrDefaultAsync(item => item.TicketId == request.TicketId);
        if (ticket is null)
        {
            throw new KeyNotFoundException("Ticket no encontrado.");
        }

        ticket.Status = request.Status;
        if (request.Status == "cerrado")
        {
            ticket.ClosedAt = DateTime.UtcNow;
        }

        unitOfWork.Tickets.Update(ticket);
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
