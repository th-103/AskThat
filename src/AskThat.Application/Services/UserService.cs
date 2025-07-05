using System;
using System.Threading.Tasks;
using AskThat.Application.Interfaces;
using AskThat.Domain.Entities;
using AskThat.Domain.Entitites;
using AskThat.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AskThat.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRepository<Role> _roleRepo;
        private readonly IPasswordHasher<User> _passwordHasher;
        private const int DefaultRoleId = 1;

        public UserService(
            IUserRepository userRepo,
            IRepository<Role> roleRepo,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _roleRepo = roleRepo ?? throw new ArgumentNullException(nameof(roleRepo));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public Task<bool> IsUsernameTakenAsync(string username)
            => _userRepo.IsUsernameExistsAsync(username);

        public Task<bool> IsEmailTakenAsync(string email)
            => _userRepo.IsEmailExistsAsync(email);

        public async Task<User> RegisterUserAsync(string username, string email, string password)
        {
            var role = await _roleRepo.GetByIdAsync(DefaultRoleId) ?? throw new InvalidOperationException($"Default role with ID {DefaultRoleId} not found.");

            var user = new User
            {
                Username = username,
                Email = email,
                RoleId = DefaultRoleId,
                Password = string.Empty,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Role = role
            };

            user.Password = _passwordHasher.HashPassword(user, password);
            await _userRepo.AddAsync(user);
            return user;
        }

        public Task<User?> GetByUsernameAsync(string username)
            => _userRepo.GetByUsernameAsync(username);

        public Task<User?> GetByEmailAsync(string email)
            => _userRepo.GetByEmailAsync(email);

        public Task<bool> CheckPasswordAsync(User user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return Task.FromResult(result == PasswordVerificationResult.Success);
        }
    }
}