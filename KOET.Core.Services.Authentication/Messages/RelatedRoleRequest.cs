namespace KOET.Core.Services.Authentication.Messages
{
    public class RelatedRoleRequest
    {
        public string SourceRole { get; set; }
        public string TargetRole { get; set; }
    }
}
