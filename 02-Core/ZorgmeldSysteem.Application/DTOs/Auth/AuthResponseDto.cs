namespace ZorgmeldSysteem.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public int UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int UserLevel { get; set; }
        public string UserLevelName { get; set; } = string.Empty;
        public List<int> CompanyIDs { get; set; } = new();
        public List<string> CompanyNames { get; set; } = new();
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiration { get; set; }
    }
}