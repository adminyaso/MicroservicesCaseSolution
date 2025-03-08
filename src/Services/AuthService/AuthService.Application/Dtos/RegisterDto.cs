namespace AuthService.Application.Dtos
{
    public class RegisterDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}