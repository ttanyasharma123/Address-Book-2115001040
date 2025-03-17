using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.Model.Entities
{
    [Table("Address")]
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)] // Limiting name length
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)] // Email length limit
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Invalid Phone Number")]
        [MaxLength(15)] // Limiting to avoid overflow
        public string Phone { get; set; }

        [Required]
        [MaxLength(500)] // Limiting address length
        public string Address { get; set; }
    }
}
