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
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AskThatDbContext _context;

        public QuestionRepository(AskThatDbContext context)
        {
            _context = context;
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await _context.Questions
                .Include(q => q.User)
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.QuestionId == id);
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.Questions
                .Include(q => q.User)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Questions.AnyAsync(q => q.QuestionId == id);
        }

        public async Task<IEnumerable<Question>> FindAsync(Expression<Func<Question, bool>> predicate)
        {
            return await _context.Questions
                .Include(q => q.User)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetQuestionsByUserAsync(int userId)
        {
            return await _context.Questions
                .Include(q => q.User)
                .Where(q => q.UserId == userId)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetRecentQuestionsAsync(int count)
        {
            return await _context.Questions
                .Include(q => q.User)
                .OrderByDescending(q => q.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Question?> GetQuestionWithAnswersAsync(int questionId)
        {
            return await _context.Questions
                .Include(q => q.User)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        }

        public async Task<IEnumerable<Question>> SearchQuestionsAsync(string searchTerm)
        {
            return await _context.Questions
                .Include(q => q.User)
                .Where(q => q.Title.Contains(searchTerm) || q.Content.Contains(searchTerm))
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateCommentCountAsync(int questionId, int commentCount)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question != null)
            {
                question.CommentCount = commentCount;
                await _context.SaveChangesAsync();
            }
        }


    }
}