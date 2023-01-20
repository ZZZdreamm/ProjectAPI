namespace ProjectAPI.Entities
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FriendId { get; set; }

        public string SenderName { get; set; }
        public string? SenderProfileImage { get; set; }

        public string FriendName { get; set; }
        public string? FriendProfileImage { get; set; }
    }
}
