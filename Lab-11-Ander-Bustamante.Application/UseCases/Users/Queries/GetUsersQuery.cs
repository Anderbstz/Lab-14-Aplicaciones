using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Users.Queries;

public record GetUsersQuery() : IRequest<List<UserDto>>;

internal sealed class GetUsersQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = (await unitOfWork.Users.GetAllAsync()).OrderBy(user => user.Username).ToList();
        var roles = (await unitOfWork.Roles.GetAllAsync()).ToDictionary(role => role.RoleId, role => role.RoleName);
        var userRoles = (await unitOfWork.UserRoles.GetAllAsync()).ToList();

        var result = new List<UserDto>();
        foreach (var user in users)
        {
            var roleNames = new List<string>();
            foreach (var userRole in userRoles.Where(item => item.UserId == user.UserId))
            {
                if (roles.TryGetValue(userRole.RoleId, out var roleName))
                {
                    roleNames.Add(roleName);
                }
            }

            result.Add(new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email ?? string.Empty,
                CreatedAt = user.CreatedAt,
                Roles = roleNames
            });
        }

        return result;
    }
}
