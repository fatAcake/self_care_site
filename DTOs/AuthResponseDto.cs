namespace Self_care.DTOs
{
    public class AuthResponseDto
    {
        public string access_token { get; set; } = string.Empty;
        public int user_id { get; set; }
        public string nickname { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string timezone { get; set; } = string.Empty;
    }
}
