using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskThat.Domain.Entitites;

namespace AskThat.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> IsUsernameTakenAsync(string username);
        Task<bool> IsEmailTakenAsync(string email);
        Task<User> RegisterUserAsync(string username, string email, string password);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(User user, string password);

    }
}