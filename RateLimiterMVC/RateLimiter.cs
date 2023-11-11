using RateLimiterMVC.Interfaces;
using RateLimiterMVC.Models;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace RateLimiterMVC
{
    public class RateLimiter : IRateLimiter
    {
        // const keyword static class wise
        // readonly can be initialized in constructor also. during runtime and compile time.

        // we store key : policy in memory
        private readonly ConcurrentDictionary<string, RateLimitingPolicy> rateLimitPolicyDictionary;
        private readonly ConcurrentDictionary<string, IRateLimitingBucket> inMemoryRateLimitBuckets;
        private readonly ISystemClock systemClock;
        private RateLimitingAlgorithm currentRateLimitAlgorithm = RateLimitingAlgorithm.FixedWindowBucket;

        public RateLimiter(RateLimitingPolicyList rateLimitingPolicyList, ISystemClock systemClock)
        {
            this.rateLimitPolicyDictionary = new ConcurrentDictionary<string, RateLimitingPolicy>(StringComparer.InvariantCultureIgnoreCase);
            this.inMemoryRateLimitBuckets = new ConcurrentDictionary<string, IRateLimitingBucket>();
            this.systemClock = systemClock;
            this.UpdateRequestLimitDictionary(rateLimitingPolicyList);
        }

        public void ChangeRateLimitingAlgorithm(RateLimitingAlgorithm updatedRateLimitingAlgorithm)
        {
            this.currentRateLimitAlgorithm = updatedRateLimitingAlgorithm;
        }

        public RateLimitingPolicy GetPolicy(string key)
        {
            // key e.h UserId$Ip$userIdGuid:Ipasddress where UserId$Ip$ is keyType and keyIdentitifier is userIdGuid:Ipasddress
            if (rateLimitPolicyDictionary.TryGetValue(key, out RateLimitingPolicy policy))
            {
                // Exact match UserId$GUID1 or UserId$Ip$
                return policy;
            }

            int lastIndexOfDollar = key.LastIndexOf('$');
            string keyPrefix = key.Substring(0, lastIndexOfDollar + 1);
            // UserId$Ip$

            if(!this.rateLimitPolicyDictionary.ContainsKey(keyPrefix))
            {
                return this.GetDefaultRateLimtingPolicy();
            }

            return this.rateLimitPolicyDictionary[keyPrefix];
            
        }

        private RateLimitingPolicy GetDefaultRateLimtingPolicy()
        {
            RateLimitingPolicy policy = new RateLimitingPolicy();
            policy.RateLimitKeyType = "Default$";
            policy.RateLimitIdentifier = "";
            policy.TotalTokenCount = 20;
            policy.DurationInSeconds = 10;

            return policy;
        }

        public bool ShouldThrottleRequest(string key, int requestCount)
        {
            IRateLimitingBucket rateLimitingBucket  = this.GetRateLimitingBucket(key);
            return rateLimitingBucket.UpdateBucketAndGetDecision(requestCount);
        }

        private IRateLimitingBucket GetRateLimitingBucket(string key)
        {
            IRateLimitingBucket rateLimitingBucket;
            if (this.inMemoryRateLimitBuckets.ContainsKey(key))
            {
                return this.inMemoryRateLimitBuckets[key];
            }
           
            if (this.currentRateLimitAlgorithm == RateLimitingAlgorithm.FixedWindowBucket)
            {
                rateLimitingBucket = new FixedWindowBucket(this.GetPolicy(key), this.systemClock);
            }
            else
            {
                // Any other bucket algo like LeakyBucket
                rateLimitingBucket = new FixedWindowBucket(this.GetPolicy(key), this.systemClock);
            }

            this.inMemoryRateLimitBuckets[key] = rateLimitingBucket;
            return rateLimitingBucket;
        }

        private void UpdateRequestLimitDictionary(RateLimitingPolicyList policiesFromConfig)
        {
            // Update dictionary
            foreach (var policy in policiesFromConfig.RequestRateLimits)
            {
                this.rateLimitPolicyDictionary.AddOrUpdate(policy.RateLimitKeyType + policy.RateLimitIdentifier, policy, (key, oldValue) => policy);
            }
        }
    }
}
