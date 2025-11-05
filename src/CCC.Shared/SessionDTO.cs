namespace CCC.Shared
{
    public class SessionDTO
    {
        public required string UId { get; set; }
        public required string Email { get; set; }
        public List<string> Providers = new List<string>();
        public string? IdToken { get; set; }
    }
}
