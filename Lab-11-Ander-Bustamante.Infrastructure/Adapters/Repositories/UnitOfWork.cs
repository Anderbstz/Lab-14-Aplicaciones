using Lab_11_Ander_Bustamante.Domain.Models;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using Lab_11_Ander_Bustamante.Infrastructure.Context;

namespace Lab_11_Ander_Bustamante.Infrastructure.Adapters;

public class UnitOfWork(Lab11DbContext context) : IUnitOfWork
{
    public IRepository<User> Users { get; } = new GenericRepository<User>(context);
    public IRepository<Role> Roles { get; } = new GenericRepository<Role>(context);
    public IRepository<UserRole> UserRoles { get; } = new GenericRepository<UserRole>(context);
    public IRepository<Ticket> Tickets { get; } = new GenericRepository<Ticket>(context);
    public IRepository<Response> Responses { get; } = new GenericRepository<Response>(context);

    public Task<int> CompleteAsync() => context.SaveChangesAsync();
}
