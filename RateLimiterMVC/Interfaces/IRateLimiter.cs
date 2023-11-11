using RateLimiterMVC.Models;

namespace RateLimiterMVC.Interfaces
{
    public interface IRateLimiter
    {
        public bool ShouldThrottleRequest(string key, int requestCount);
    }
}
