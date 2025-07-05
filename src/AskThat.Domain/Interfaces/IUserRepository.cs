using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskThat.Domain.Entities;
using AskThat.Domain.Interfaces;
using AskThat.Domain.Entitites;
using System.Linq.Expressions;

namespace AskThat.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {

        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<bool> IsEmailExistsAsync(string email);
        Task<IEnumerable<User>> GetActiveUsersAsync();

    }
}