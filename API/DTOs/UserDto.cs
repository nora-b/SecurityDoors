namespace API.DTOs
{
    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public bool InOffice { get; set; }
    }
}