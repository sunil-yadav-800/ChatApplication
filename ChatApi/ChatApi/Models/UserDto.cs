using System.ComponentModel.DataAnnotations;

namespace ChatApi.Models
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int UnreadMessages { get; set; }
    }
}
