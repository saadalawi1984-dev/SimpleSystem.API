namespace SimpleSystem.DataAccess.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public int PersonId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
