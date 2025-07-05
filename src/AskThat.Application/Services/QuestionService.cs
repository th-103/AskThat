using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskThat.Application.Interfaces;
using AskThat.Domain.Entities;
using AskThat.Domain.Interfaces;

namespace AskThat.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepo;

        public QuestionService(IQuestionRepository questionRepo)
        {
            _questionRepo = questionRepo ?? throw new ArgumentNullException(nameof(questionRepo));
        }

        public async Task<IEnumerable<Question>> GetAllQuestionsAsync(int page = 1, int limit = 20, string? search = null)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var searchResults = await _questionRepo.SearchQuestionsAsync(search);
                return searchResults.Skip((page - 1) * limit).Take(limit);
            }

            var allQuestions = await _questionRepo.GetAllAsync();
            return allQuestions.Skip((page - 1) * limit).Take(limit);
        }

        public Task<Question?> GetQuestionByIdAsync(int id)
            => _questionRepo.GetByIdAsync(id);

        public async Task<Question> CreateQuestionAsync(int userId, string title, string content)
        {
            var question = new Question
            {
                UserId = userId,
                Title = title,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                CommentCount = 0
            };

            await _questionRepo.AddAsync(question);
            return question;
        }

        public async Task<Question?> UpdateQuestionAsync(int id, int userId, string title, string content)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null || question.UserId != userId)
                return null;

            question.Title = title;
            question.Content = content;
            question.UpdateAt = DateTime.UtcNow;

            await _questionRepo.UpdateAsync(question);
            return question;
        }

        public async Task<bool> DeleteQuestionAsync(int id, int userId)
        {
            var question = await _questionRepo.GetByIdAsync(id);
            if (question == null || question.UserId != userId)
                return false;

            await _questionRepo.DeleteAsync(id);
            return true;
        }

        public Task<Question?> GetQuestionWithAnswersAsync(int id)
            => _questionRepo.GetQuestionWithAnswersAsync(id);
    }
}