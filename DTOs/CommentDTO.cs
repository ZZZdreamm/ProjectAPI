using ProjectAPI.Entities;

namespace ProjectAPI.DTOs
{
    public class CommentDTO
    {
        public CommentDTO(int id, int postId, string autorName, string autorProfileImage, string textContent)
        {
            Id = id;
            PostId = postId;
            AutorName = autorName;
            AutorProfileImage = autorProfileImage;
            TextContent = textContent;
        }

        public int Id { get; set; }
        public int PostId { get; set; }
        public string AutorName { get; set; }
        public string AutorProfileImage { get; set; }
        public string TextContent { get; set; }
    }
}
