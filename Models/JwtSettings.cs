namespace Self_care.Models
{
    public record JwtSettings
    {
        public required string Key { get; init; }
        public string Issuer { get; init; } = "SelfCare.Api";
        public string Audience { get; init; } = "SelfCare.Front";
        public int ExpiryMinutes { get; init; } = 60;
    }
}