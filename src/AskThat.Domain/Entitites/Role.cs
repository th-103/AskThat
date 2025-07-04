using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using AskThat.Domain.Entitites;

namespace AskThat.Domain.Entities
{
    public class Role
    {
        public int RoleId { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}