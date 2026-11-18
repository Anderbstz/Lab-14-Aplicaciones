using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lab_11_Ander_Bustamante.Application.DTOs;
using Lab_11_Ander_Bustamante.Application.Services;
using Lab_11_Ander_Bustamante.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Lab_11_Ander_Bustamante.Infrastructure.Adapters.Services;

public class AuthService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IConfiguration configuration) : IAuthService
{
    public async Task<LoginResponse> LoginAsync(string username, string password)
    {
        var user = await unitOfWork.Users.FirstOrDefaultAsync(item => item.Username == username);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Credenciales invalidas.");
        }

        if (!passwordHasher.Verify(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Credenciales invalidas.");
        }

        var roleNames = await GetRoleNamesAsync(user.UserId);

        var secretKey = configuration["Jwt:SecretKey"];
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException("La configuracion de JWT esta incompleta.");
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(GetExpirationMinutes());

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username)
        };

        foreach (var roleName in roleNames)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return new LoginResponse
        {
            Token = tokenValue,
            ExpiresAt = expiresAt
        };
    }

    private async Task<List<string>> GetRoleNamesAsync(Guid userId)
    {
        var roles = (await unitOfWork.Roles.GetAllAsync()).ToDictionary(role => role.RoleId, role => role.RoleName);
        var userRoles = await unitOfWork.UserRoles.FindAsync(item => item.UserId == userId);

        var roleNames = new List<string>();
        foreach (var userRole in userRoles)
        {
            if (roles.TryGetValue(userRole.RoleId, out var roleName))
            {
                roleNames.Add(roleName);
            }
        }

        return roleNames;
    }

    private int GetExpirationMinutes()
    {
        var value = configuration["Jwt:ExpiraMinutos"];
        if (int.TryParse(value, out var minutes) && minutes > 0)
        {
            return minutes;
        }

        return 60;
    }
}
