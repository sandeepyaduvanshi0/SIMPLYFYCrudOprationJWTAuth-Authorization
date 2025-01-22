using Microsoft.Win32;
using Server.Models.DTOs;

namespace Server.Repository.IRepository
{
    public interface IUserAccount
    {
        Task<GeneralResponse> RegisterAsync(Register user);
        Task<LoginResponse> LoginAsync(Login user);      
    }
}
