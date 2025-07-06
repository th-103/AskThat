using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace AskThat.Web.Models.ViewModels
{
    public class AnswerViewModel
    {
        public int AnswerId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int QuestionId { get; set; }
    }

    public class CreateAnswerViewModel
    {
        [Required(ErrorMessage = "Answer content is required")]
        [StringLength(10000, ErrorMessage = "Answer cannot exceed 10000 characters")]
        [MinLength(10, ErrorMessage = "Answer must be at least 10 characters long")]
        public string Content { get; set; } = string.Empty;

        public int QuestionId { get; set; }
    }

    public class EditAnswerViewModel
    {
        public int AnswerId { get; set; }

        [Required(ErrorMessage = "Answer content is required")]
        [StringLength(10000, ErrorMessage = "Answer cannot exceed 10000 characters")]
        [MinLength(10, ErrorMessage = "Answer must be at least 10 characters long")]
        public string Content { get; set; } = string.Empty;

        public int QuestionId { get; set; }
    }
}