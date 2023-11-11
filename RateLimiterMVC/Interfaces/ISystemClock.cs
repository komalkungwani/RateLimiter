namespace RateLimiterMVC.Interfaces
{
    public interface ISystemClock
    {
        public DateTime UtcNow();
    }
}
