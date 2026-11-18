using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Tickets.Queries;

public record GetTicketByIdQuery(Guid TicketId) : IRequest<TicketDto>;

internal sealed class GetTicketByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTicketByIdQuery, TicketDto>
{
    public async Task<TicketDto> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var ticket = await unitOfWork.Tickets.FirstOrDefaultAsync(item => item.TicketId == request.TicketId);
        if (ticket is null)
        {
            throw new KeyNotFoundException("Ticket no encontrado.");
        }

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
