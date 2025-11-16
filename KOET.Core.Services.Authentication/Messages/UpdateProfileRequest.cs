namespace KOET.Core.Services.Authentication.Messages
{
    public class UpdateProfileRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public IFormFile? Photo { get; set; } // Optional image upload
    }
}
