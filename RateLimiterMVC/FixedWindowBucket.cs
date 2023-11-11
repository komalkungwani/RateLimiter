using Microsoft.AspNetCore.Authentication;
using RateLimiterMVC.Interfaces;
using RateLimiterMVC.Models;
using ISystemClock = RateLimiterMVC.Interfaces.ISystemClock;

namespace RateLimiterMVC
{
    public class FixedWindowBucket : IRateLimitingBucket
    {

        private readonly object lockObject = new object();
        private int availableTokens;
        private RateLimitingPolicy rateLimitingPolicy;
        private DateTime lastBucketUpdateTime;
        private ISystemClock systemClock;

        public FixedWindowBucket(RateLimitingPolicy rateLimitingPolicy, ISystemClock systemClock)
        {
            this.availableTokens = rateLimitingPolicy.TotalTokenCount;
            this.rateLimitingPolicy = rateLimitingPolicy;
            this.systemClock = systemClock;
            this.lastBucketUpdateTime = this.systemClock.UtcNow();
        }


        public bool UpdateBucketAndGetDecision(int count)
        {
            lock(this.lockObject)
            {
                var currentDateTime = this.systemClock.UtcNow();
                bool shouldReplenish = this.ShouldReplenish(currentDateTime);

                if(shouldReplenish)
                {
                    this.availableTokens = this.rateLimitingPolicy.TotalTokenCount;
                    this.lastBucketUpdateTime = currentDateTime;
                }

                if(this.availableTokens - count >= 0)
                {
                    this.availableTokens -= count;
                    return true;
                }

                return false;
            }
        }

        private bool ShouldReplenish(DateTime currentDateTime)
        {
            return (currentDateTime - this.lastBucketUpdateTime).TotalSeconds >= this.rateLimitingPolicy.DurationInSeconds;
        }


    }
}
