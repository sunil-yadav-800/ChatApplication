using System.ComponentModel.DataAnnotations;

namespace ChatApi.Models
{
    public class Registeration
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
