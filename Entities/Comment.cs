namespace ProjectAPI.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string AutorId { get; set; }
        public string TextContent { get; set; }
    }
}
