using Lab_11_Ander_Bustamante.Domain.Repositories;
using MediatR;

namespace Lab_11_Ander_Bustamante.Application.UseCases.Users.Commands;

public record DeleteUserCommand(Guid UserId) : IRequest;

internal sealed class DeleteUserCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.FirstOrDefaultAsync(item => item.UserId == request.UserId);
        if (user is null)
        {
            throw new KeyNotFoundException("Usuario no encontrado.");
        }

        var userRoles = await unitOfWork.UserRoles.FindAsync(item => item.UserId == request.UserId);
        foreach (var userRole in userRoles)
        {
            unitOfWork.UserRoles.Delete(userRole);
        }

        unitOfWork.Users.Delete(user);
        await unitOfWork.CompleteAsync();
    }
}
