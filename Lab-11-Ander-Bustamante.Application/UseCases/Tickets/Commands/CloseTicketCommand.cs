using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Tickets.Commands;

public record CloseTicketCommand(Guid TicketId) : IRequest<TicketDto>;

internal sealed class CloseTicketCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CloseTicketCommand, TicketDto>
{
    public async Task<TicketDto> Handle(CloseTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await unitOfWork.Tickets.FirstOrDefaultAsync(item => item.TicketId == request.TicketId);
        if (ticket is null)
        {
            throw new KeyNotFoundException("Ticket no encontrado.");
        }

        ticket.Status = "cerrado";
        ticket.ClosedAt = DateTime.UtcNow;

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
