using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Users.Commands;

public record UpdateUserCommand(Guid UserId, string Username, string Email) : IRequest<UserDto>;

internal sealed class UpdateUserCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new InvalidOperationException("El nombre de usuario es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new InvalidOperationException("El correo es obligatorio.");
        }

        var user = await unitOfWork.Users.FirstOrDefaultAsync(item => item.UserId == request.UserId);
        if (user is null)
        {
            throw new KeyNotFoundException("Usuario no encontrado.");
        }

        var existingUser = await unitOfWork.Users.FirstOrDefaultAsync(item =>
            item.Username == request.Username && item.UserId != request.UserId);

        if (existingUser is not null)
        {
            throw new InvalidOperationException("El nombre de usuario ya existe.");
        }

        user.Username = request.Username;
        user.Email = request.Email;
        unitOfWork.Users.Update(user);
        await unitOfWork.CompleteAsync();

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
