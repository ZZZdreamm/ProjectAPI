namespace ProjectAPI.DTOs
{
    public class PostCreationDTO
    {
        public string AutorName { get; set; }
        public string TextContent { get; set; }
        public IFormFile? MediaFile { get; set; }

    }
}
