using Domain;

namespace API.DTOs
{
    public class UpdateTagDto
    {
        public TagStatus? TagStatusInTunnel { get; set; }
        public bool AuthorizeInTunnel { get; set; }
        public TagStatus? TagStatusInOffice { get; set; }
        public bool AuthorizeInOffice { get; set; }
        public string Username { get; set; }
    }
}