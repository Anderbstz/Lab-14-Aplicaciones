using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Domain.Models;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Roles.Commands;

public record CreateRoleCommand(string RoleName) : IRequest<RoleDto>;

internal sealed class CreateRoleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRoleCommand, RoleDto>
{
    public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RoleName))
        {
            throw new InvalidOperationException("El nombre del rol es obligatorio.");
        }

        var existingRole = await unitOfWork.Roles.FirstOrDefaultAsync(role => role.RoleName == request.RoleName);
        if (existingRole is not null)
        {
            throw new InvalidOperationException("El rol ya existe.");
        }

        var role = new Role
        {
            RoleId = Guid.NewGuid(),
            RoleName = request.RoleName
        };

        await unitOfWork.Roles.AddAsync(role);
        await unitOfWork.CompleteAsync();

        return new RoleDto
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName
        };
    }
}
