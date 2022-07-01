namespace Domain
{
    public class UserHistory
    {
        public Guid Id { get; set; }
        public DateTime LastLoggedInOffice { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}