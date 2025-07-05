// src/AskThat.Application/Interfaces/IQuestionService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AskThat.Domain.Entities;

namespace AskThat.Application.Interfaces
{
    public interface IQuestionService
    {
        Task<IEnumerable<Question>> GetAllQuestionsAsync(int page = 1, int limit = 20, string? search = null);
        Task<Question?> GetQuestionByIdAsync(int id);
        Task<Question> CreateQuestionAsync(int userId, string title, string content);
        Task<Question?> UpdateQuestionAsync(int id, int userId, string title, string content);
        Task<bool> DeleteQuestionAsync(int id, int userId);
        Task<Question?> GetQuestionWithAnswersAsync(int id);
    }
}