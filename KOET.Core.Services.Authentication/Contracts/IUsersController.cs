using Microsoft.AspNetCore.Mvc;

namespace KOET.Core.Services.Authentication.Contracts
{
    public interface IUsersController
    {
        Task<IActionResult> GetAllUsers();
    }
}