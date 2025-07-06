using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskThat.Domain.Entities;

namespace AskThat.Application.Interfaces
{
    public interface IAnswerService
    {
        Task<IEnumerable<Answer>> GetAnswersByQuestionAsync(int questionId);
        Task<Answer?> GetAnswerByIdAsync(int answerId);
        Task<Answer> CreateAnswerAsync(int userId, int questionId, string content);
        Task<Answer?> UpdateAnswerAsync(int answerId, int userId, string content);
        Task<bool> DeleteAnswerAsync(int answerId, int userId);
        Task<int> GetAnswerCountByQuestionAsync(int questionId);
    }
}