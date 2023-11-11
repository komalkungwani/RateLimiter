namespace RateLimiterMVC.Models
{
    public class RateLimitingPolicyList
    {
        public int Version { get; set; }

        public List<RateLimitingPolicy> RequestRateLimits { get; set; } = new List<RateLimitingPolicy>();
    }
}
