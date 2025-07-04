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
    public class AnswerRepository : IAnswerRepository
    {
        private readonly AskThatDbContext _context;

        public AnswerRepository(AskThatDbContext context)
        {
            _context = context;
        }

        public async Task<Answer?> GetByIdAsync(int id)
        {
            return await _context.Answers
                .Include(a => a.User)
                .Include(a => a.Question)
                .FirstOrDefaultAsync(a => a.AnswerId == id);
        }

        public async Task<IEnumerable<Answer>> GetAllAsync()
        {
            return await _context.Answers
                .Include(a => a.User)
                .Include(a => a.Question)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Answer answer)
        {
            await _context.Answers.AddAsync(answer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Answer answer)
        {
            _context.Answers.Update(answer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var answer = await _context.Answers.FindAsync(id);
            if (answer != null)
            {
                _context.Answers.Remove(answer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Answers.AnyAsync(a => a.AnswerId == id);
        }

        public async Task<IEnumerable<Answer>> FindAsync(Expression<Func<Answer, bool>> predicate)
        {
            return await _context.Answers
                .Include(a => a.User)
                .Include(a => a.Question)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Answer>> GetAnswersByQuestionAsync(int questionId)
        {
            return await _context.Answers
                .Include(a => a.User)
                .Where(a => a.QuestionId == questionId)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Answer>> GetAnswersByUserAsync(int userId)
        {
            return await _context.Answers
                .Include(a => a.Question)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetAnswerCountByQuestionAsync(int questionId)
        {
            return await _context.Answers.CountAsync(a => a.QuestionId == questionId);
        }

        Task<IEnumerable<Answer>> IAnswerRepository.GetAnswersByQuestionAsync(int questionId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Answer>> IAnswerRepository.GetAnswersByUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        Task<Answer?> IRepository<Answer>.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Answer>> IRepository<Answer>.GetAllAsync()
        {
            throw new NotImplementedException();
        }


    }
}