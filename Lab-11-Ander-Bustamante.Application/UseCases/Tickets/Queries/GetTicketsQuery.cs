using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Tickets.Queries;

public record GetTicketsQuery(string? Status) : IRequest<List<TicketDto>>;

internal sealed class GetTicketsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetTicketsQuery, List<TicketDto>>
{
    public async Task<List<TicketDto>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
    {
        var tickets = await unitOfWork.Tickets.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            tickets = tickets.Where(ticket => ticket.Status == request.Status);
        }

        var result = new List<TicketDto>();
        foreach (var ticket in tickets.OrderByDescending(ticket => ticket.CreatedAt))
        {
            result.Add(new TicketDto
            {
                TicketId = ticket.TicketId,
                UserId = ticket.UserId,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status,
                CreatedAt = ticket.CreatedAt,
                ClosedAt = ticket.ClosedAt
            });
        }

        return result;
    }
}
