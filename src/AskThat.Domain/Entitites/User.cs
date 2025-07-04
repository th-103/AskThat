using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AskThat.Domain.Entities;

namespace AskThat.Domain.Entitites
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Username { get; set; }

        [Required]
        [StringLength(255)]
        public required string Password { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public required string Email { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public int RoleId { get; set; }

        public required Role Role { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }

}