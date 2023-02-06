namespace ProjectAPI.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string AutorName { get; set; }
        public string TextContent { get; set; }

        public string? MediaFile { get; set; }
        public int AmountOfComments { get; set; } = 0;
        public int AmountOfLikes { get; set; } = 0;
    }
}
