using System.ComponentModel.DataAnnotations;

namespace AskThat.Web.Models.ViewModels
{
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Username { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int AnswerCount { get; set; }

        public IEnumerable<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();

    }

    public class CreateQuestionViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
        public string Content { get; set; } = string.Empty;
    }

    public class QuestionsListViewModel
    {
        public IEnumerable<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SearchTerm { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}