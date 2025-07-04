using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AskThat.Domain.Entities;
using AskThat.Domain.Interfaces;
using AskThat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AskThat.Infrastructure.Repositories
{
    public class RoleRepository : IRepository<Role>
    {
        private readonly AskThatDbContext _context;

        public RoleRepository(AskThatDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task AddAsync(Role entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _context.Roles.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Roles.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Roles.FindAsync(id);
            if (entity != null)
            {
                _context.Roles.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int RoleId)
        {
            return await _context.Roles.AnyAsync(r => r.RoleId == RoleId);
        }

        public async Task<IEnumerable<Role>> FindAsync(Expression<Func<Role, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _context.Roles.Where(predicate).ToListAsync();
        }
    }
}