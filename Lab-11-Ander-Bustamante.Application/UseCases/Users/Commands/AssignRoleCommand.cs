using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Domain.Models;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Users.Commands;

public record AssignRoleCommand(Guid UserId, Guid RoleId) : IRequest<UserDto>;

internal sealed class AssignRoleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AssignRoleCommand, UserDto>
{
    public async Task<UserDto> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.FirstOrDefaultAsync(item => item.UserId == request.UserId);
        if (user is null)
        {
            throw new KeyNotFoundException("Usuario no encontrado.");
        }

        var role = await unitOfWork.Roles.FirstOrDefaultAsync(item => item.RoleId == request.RoleId);
        if (role is null)
        {
            throw new KeyNotFoundException("Rol no encontrado.");
        }

        var existingAssignment = await unitOfWork.UserRoles.FirstOrDefaultAsync(item =>
            item.UserId == request.UserId && item.RoleId == request.RoleId);

        if (existingAssignment is null)
        {
            await unitOfWork.UserRoles.AddAsync(new UserRole
            {
                UserId = request.UserId,
                RoleId = request.RoleId,
                AssignedAt = DateTime.UtcNow
            });
            await unitOfWork.CompleteAsync();
        }

        var roles = (await unitOfWork.Roles.GetAllAsync()).ToDictionary(item => item.RoleId, item => item.RoleName);
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
