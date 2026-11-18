using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Users.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto>;

internal sealed class GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.FirstOrDefaultAsync(item => item.UserId == request.UserId);
        if (user is null)
        {
            throw new KeyNotFoundException("Usuario no encontrado.");
        }

        var roles = (await unitOfWork.Roles.GetAllAsync()).ToDictionary(role => role.RoleId, role => role.RoleName);
        var userRoles = await unitOfWork.UserRoles.FindAsync(item => item.UserId == request.UserId);

        var roleNames = new List<string>();
        foreach (var userRole in userRoles)
        {
            if (roles.TryGetValue(userRole.RoleId, out var roleName))
            {
                roleNames.Add(roleName);
            }
        }

        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email ?? string.Empty,
            CreatedAt = user.CreatedAt,
            Roles = roleNames
        };
    }
}
