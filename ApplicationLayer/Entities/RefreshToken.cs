namespace PerfumeryBackend.ApplicationLayer.Entities
{
    public record RefreshToken(
        DateTime Expires,
        string Token)
    {
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public static RefreshToken Create(string token, DateTime expires) =>
            new(expires, token);
    }
}
