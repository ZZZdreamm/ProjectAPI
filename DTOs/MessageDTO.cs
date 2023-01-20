namespace ProjectAPI.DTOs
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string SenderId { get; set; }

        public string ReceiverId { get; set; }
        public string? TextContent { get; set; }
        public string? ImageContent { get; set; }
        public string Date { get; set; }
    }
}
