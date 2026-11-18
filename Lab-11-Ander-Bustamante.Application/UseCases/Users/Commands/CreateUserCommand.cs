using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Application.Services;
using Lab_11_Ander_Bustamante.Domain.Models;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Users.Commands;

public record CreateUserCommand(string Username, string Email, string Password, Guid? RoleId) : IRequest<UserDto>;

internal sealed class CreateUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    : IRequestHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new InvalidOperationException("El nombre de usuario es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new InvalidOperationException("El correo es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new InvalidOperationException("La contrasena es obligatoria.");
        }

        var existingUser = await unitOfWork.Users.FirstOrDefaultAsync(user => user.Username == request.Username);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("El nombre de usuario ya existe.");
        }

        Role? role = null;
        if (request.RoleId.HasValue)
        {
            role = await unitOfWork.Roles.FirstOrDefaultAsync(item => item.RoleId == request.RoleId.Value);
            if (role is null)
            {
                throw new InvalidOperationException("El rol indicado no existe.");
            }
        }

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await unitOfWork.Users.AddAsync(user);

        var roleNames = new List<string>();
        if (role is not null)
        {
            await unitOfWork.UserRoles.AddAsync(new UserRole
            {
                UserId = user.UserId,
                RoleId = role.RoleId,
                AssignedAt = DateTime.UtcNow
            });
            roleNames.Add(role.RoleName);
        }

        await unitOfWork.CompleteAsync();

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
