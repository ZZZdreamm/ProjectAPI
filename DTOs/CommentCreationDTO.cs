namespace ProjectAPI.DTOs
{
    public class CommentCreationDTO
    {
        public int PostId { get; set; }
        public string AutorId { get; set; }
        public string TextContent { get; set; }
    }
}
