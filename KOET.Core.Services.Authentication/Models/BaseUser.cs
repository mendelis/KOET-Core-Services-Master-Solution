namespace KOET.Core.Services.Authentication.Models
{
    public class BaseUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; }
        
        public List<string> Roles { get; set; } = new() { "User" };
       
        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string? PhotoUrl { get; set; }
       
    }
}
