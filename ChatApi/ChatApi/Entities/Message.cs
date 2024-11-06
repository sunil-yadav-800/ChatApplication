namespace ChatApi.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid From  { get; set; }
        public User Sender { get; set; }
        public Guid To { get; set; }
        public User Receiver { get; set; }
        public bool IsSeen { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
