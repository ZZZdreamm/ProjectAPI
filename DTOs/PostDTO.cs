namespace ProjectAPI.DTOs
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string AutorName { get; set; }
        public string TextContent { get; set; }
        public string? MediaFile { get; set; }
        public string? AutorProfileImage { get; set; }
        public int AmountOfComments { get; set; }
        public int AmountOfLikes { get; set; }
    }
}
