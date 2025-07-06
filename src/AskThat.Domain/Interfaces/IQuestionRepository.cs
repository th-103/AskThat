using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskThat.Domain.Entities;

namespace AskThat.Domain.Interfaces
{
    public interface IQuestionRepository : IRepository<Question>
    {
        Task<IEnumerable<Question>> GetQuestionsByUserAsync(int userId);
        Task<IEnumerable<Question>> GetRecentQuestionsAsync(int count);
        Task<Question?> GetQuestionWithAnswersAsync(int questionId);
        Task<IEnumerable<Question>> SearchQuestionsAsync(string searchTerm);
        Task UpdateAnswerCountAsync(int questionId, int AnswerCount);
    }
}