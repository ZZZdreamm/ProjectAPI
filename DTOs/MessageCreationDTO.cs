namespace ProjectAPI.DTOs
{
    public class MessageCreationDTO
    {
        public string SenderId { get; set; }

        public string ReceiverId { get; set; }
        public string? TextContent { get; set; }
        public List<IFormFile>? ImageContent { get; set; }
        public string Date { get; set; }
    }
}
