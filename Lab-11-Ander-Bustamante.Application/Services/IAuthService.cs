using Lab_11_Ander_Bustamante.Application.DTOs;

namespace Lab_11_Ander_Bustamante.Application.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(string username, string password);
}
