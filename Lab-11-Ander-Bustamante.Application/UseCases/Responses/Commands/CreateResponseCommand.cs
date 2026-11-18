using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Domain.Models;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Responses.Commands;

public record CreateResponseCommand(Guid TicketId, Guid ResponderId, string Message) : IRequest<ResponseDto>;

internal sealed class CreateResponseCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateResponseCommand, ResponseDto>
{
    public async Task<ResponseDto> Handle(CreateResponseCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            throw new InvalidOperationException("El mensaje es obligatorio.");
        }

        var ticket = await unitOfWork.Tickets.FirstOrDefaultAsync(item => item.TicketId == request.TicketId);
        if (ticket is null)
        {
            throw new InvalidOperationException("El ticket indicado no existe.");
        }

        var responder = await unitOfWork.Users.FirstOrDefaultAsync(item => item.UserId == request.ResponderId);
        if (responder is null)
        {
            throw new InvalidOperationException("El usuario que responde no existe.");
        }

        var response = new Response
        {
            ResponseId = Guid.NewGuid(),
            TicketId = request.TicketId,
            ResponderId = request.ResponderId,
            Message = request.Message,
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.Responses.AddAsync(response);
        await unitOfWork.CompleteAsync();

        return new ResponseDto
        {
            ResponseId = response.ResponseId,
            TicketId = response.TicketId,
            ResponderId = response.ResponderId,
            Message = response.Message,
            CreatedAt = response.CreatedAt
        };
    }
}
