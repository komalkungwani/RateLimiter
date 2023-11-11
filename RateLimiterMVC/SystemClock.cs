using ISystemClock = RateLimiterMVC.Interfaces.ISystemClock;

namespace RateLimiterMVC
{
    public class SystemClock : ISystemClock
    {
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
