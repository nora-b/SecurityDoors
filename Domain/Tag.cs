namespace Domain
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public TagStatus StatusTunnel { get; set; }
        public bool IsAuthorizedTunnel { get; set; }
        public TagStatus StatusOffice  { get; set; }
        public bool IsAuthorizedOffice { get; set; }
        public DateTimeOffset TagTunnelExpiresAt { get; set; }
        public DateTimeOffset TagOfficeExpiresAt { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
        
    }
}