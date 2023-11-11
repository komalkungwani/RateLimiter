using RateLimiterMVC.Models;

namespace RateLimiterMVC.Interfaces
{
    public interface IRateLimitingBucket
    {
        bool UpdateBucketAndGetDecision(int count);
    }
}
