using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Model.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        public string Role { get; set; } = "User"; // Default role is "User"

        // Navigation Property for Contacts (One-to-Many)
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    }
}
