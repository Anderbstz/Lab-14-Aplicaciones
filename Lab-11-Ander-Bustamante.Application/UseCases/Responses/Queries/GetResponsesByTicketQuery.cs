using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Responses.Queries;

public record GetResponsesByTicketQuery(Guid TicketId) : IRequest<List<ResponseDto>>;

internal sealed class GetResponsesByTicketQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetResponsesByTicketQuery, List<ResponseDto>>
{
    public async Task<List<ResponseDto>> Handle(GetResponsesByTicketQuery request, CancellationToken cancellationToken)
    {
        var responses = await unitOfWork.Responses.FindAsync(response => response.TicketId == request.TicketId);

        var result = new List<ResponseDto>();
        foreach (var response in responses.OrderBy(item => item.CreatedAt))
        {
            result.Add(new ResponseDto
            {
                ResponseId = response.ResponseId,
                TicketId = response.TicketId,
                ResponderId = response.ResponderId,
                Message = response.Message,
                CreatedAt = response.CreatedAt
            });
        }

        return result;
    }
}
