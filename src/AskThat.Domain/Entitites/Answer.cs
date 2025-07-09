using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using AskThat.Domain.Entitites;

namespace AskThat.Domain.Entities
{
    public class Answer
    {
        public int AnswerId { get; set; }

        public int QuestionId { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        [StringLength(5000)]
        public string Content { get; set; } = string.Empty;

        public Question Question { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}