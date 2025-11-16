namespace KOET.Core.Services.Authentication.Models
{
    public class Permission
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
    }

}
