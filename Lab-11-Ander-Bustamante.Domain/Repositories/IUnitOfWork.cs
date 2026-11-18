using Lab_11_Ander_Bustamante.Domain.Models;

namespace Lab_11_Ander_Bustamante.Domain.Repositories;

public interface IUnitOfWork
{
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    IRepository<UserRole> UserRoles { get; }
    IRepository<Ticket> Tickets { get; }
    IRepository<Response> Responses { get; }

    Task<int> CompleteAsync();
}
