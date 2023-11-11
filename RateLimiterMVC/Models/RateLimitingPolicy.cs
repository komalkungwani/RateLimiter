namespace RateLimiterMVC.Models
{
    public class RateLimitingPolicy
    {
        public RateLimitingPolicy()
        {

        }

        public string RateLimitKeyType { get; set; }

        public string RateLimitIdentifier { get; set; }

        public int DurationInSeconds { get; set; }

        public int TotalTokenCount { get; set; }
    }
}
