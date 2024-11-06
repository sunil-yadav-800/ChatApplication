namespace ChatApi.Models
{
    public class MessageDto
    {
        public string Id { get; set; }
        public string? Content { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public bool? IsSeen { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
