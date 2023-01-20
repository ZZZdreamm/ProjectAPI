namespace ProjectAPI.DTOs
{
    public class ProfileAddImageDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }

        public IFormFile? ProfileImage { get; set; }
    }
}
