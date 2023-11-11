namespace RateLimiterMVC.Models
{
    public enum RateLimitKeyType
    {
        UserId,

        IpAddress,

        UserIdIpAddress,

        SessionId
    }
}
