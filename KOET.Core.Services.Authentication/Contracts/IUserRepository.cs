using KOET.Core.Services.Authentication.Models;

namespace KOET.Core.Services.Authentication.Contracts
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByTokenAsync(string token);
        Task<User> GetByIdAsync(string id);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task RevokeTokenAsync(string userId, string token);
        Task<bool> IsTokenRevokedAsync(string userId, string token);
    }

}
