namespace KOET.Core.Services.Authentication.Models
{
    public class Role
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
    }

}
