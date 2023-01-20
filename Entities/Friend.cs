namespace ProjectAPI.Entities
{
    public class Friend
    {
        public string Id { get; set; }
        public string Email { get; set; }

        public string? ProfileImage { get; set; }
        public List<ProfilesFriends> ProfilesFriends { get; set; }

    }
}
