using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Roles.Queries;

public record GetRolesQuery() : IRequest<List<RoleDto>>;

internal sealed class GetRolesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetRolesQuery, List<RoleDto>>
{
    public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = (await unitOfWork.Roles.GetAllAsync()).OrderBy(role => role.RoleName);

        var result = new List<RoleDto>();
        foreach (var role in roles)
        {
            result.Add(new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            });
        }

        return result;
    }
}
