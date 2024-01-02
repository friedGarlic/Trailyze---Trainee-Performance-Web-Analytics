using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ML_ASP.Models
{
    public class Account_Model
    {
        [Key]
        public Guid Id { get; set; }

        [Range(18,50)]
        public int Age { get; set; }

        [DisplayName("Full Name")]
        public string FullName { get; set; }

        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
