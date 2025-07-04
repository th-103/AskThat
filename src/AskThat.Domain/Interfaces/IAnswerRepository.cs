using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AskThat.Domain.Entities;

namespace AskThat.Domain.Interfaces
{
    public interface IAnswerRepository : IRepository<Answer>
    {
        Task<IEnumerable<Answer>> GetAnswersByQuestionAsync(int questionId);
        Task<IEnumerable<Answer>> GetAnswersByUserAsync(int userId);
        Task<int> GetAnswerCountByQuestionAsync(int questionId);
    }
}