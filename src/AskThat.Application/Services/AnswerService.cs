using AskThat.Application.Interfaces;
using AskThat.Domain.Entities;
using AskThat.Domain.Interfaces;

namespace AskThat.Application.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IQuestionRepository _questionRepository;

        public AnswerService(IAnswerRepository answerRepository, IQuestionRepository questionRepository)
        {
            _answerRepository = answerRepository ?? throw new ArgumentNullException(nameof(answerRepository));
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
        }

        public async Task<IEnumerable<Answer>> GetAnswersByQuestionAsync(int questionId)
        {
            return await _answerRepository.GetAnswersByQuestionAsync(questionId);
        }

        public async Task<Answer?> GetAnswerByIdAsync(int answerId)
        {
            return await _answerRepository.GetByIdAsync(answerId);
        }

        public async Task<Answer> CreateAnswerAsync(int userId, int questionId, string content)
        {
            // Verify question exists
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                throw new ArgumentException("Question not found", nameof(questionId));

            var answer = new Answer
            {
                UserId = userId,
                QuestionId = questionId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _answerRepository.AddAsync(answer);

            // Update question's answer count
            var answerCount = await _answerRepository.GetAnswerCountByQuestionAsync(questionId);
            question.AnswerCount = answerCount;

            await _questionRepository.UpdateAsync(question);

            return answer;
        }

        public async Task<Answer?> UpdateAnswerAsync(int answerId, int userId, string content)
        {
            var answer = await _answerRepository.GetByIdAsync(answerId);
            if (answer == null || answer.UserId != userId)
                return null;

            answer.Content = content;
            answer.UpdatedAt = DateTime.UtcNow;

            await _answerRepository.UpdateAsync(answer);
            return answer;
        }

        public async Task<bool> DeleteAnswerAsync(int answerId, int userId)
        {
            var answer = await _answerRepository.GetByIdAsync(answerId);
            if (answer == null || answer.UserId != userId)
                return false;

            var questionId = answer.QuestionId;

            // Get the question before deleting the answer
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
                return false;

            await _answerRepository.DeleteAsync(answerId);

            // Update question's answer count
            var answerCount = await _answerRepository.GetAnswerCountByQuestionAsync(questionId);
            question.AnswerCount = answerCount;
            question.UpdatedAt = DateTime.UtcNow;

            await _questionRepository.UpdateAsync(question);

            return true;
        }

        public async Task<int> GetAnswerCountByQuestionAsync(int questionId)
        {
            return await _answerRepository.GetAnswerCountByQuestionAsync(questionId);
        }
    }
}