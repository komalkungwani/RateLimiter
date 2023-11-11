using Newtonsoft.Json;
using RateLimiterMVC.Interfaces;
using RateLimiterMVC.Models;

namespace RateLimiterMVC
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            /*var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();*/

            string json = await File.ReadAllTextAsync("RateLimitingConfigs.json");
            RateLimitingPolicyList rateLimitPolicyList = JsonConvert.DeserializeObject<RateLimitingPolicyList>(json);
            if (rateLimitPolicyList == null) 
            {
                Console.WriteLine("null configs");
                return;
            }

            ISystemClock clock = new SystemClock();
            IRateLimiter rateLimiter = new RateLimiter(rateLimitPolicyList, clock);
            Console.WriteLine($"First Response : {rateLimiter.ShouldThrottleRequest("UserId$1234:", 21)}");
            Console.WriteLine($"Second Response : {rateLimiter.ShouldThrottleRequest("UserId$1234:", 20)}");
            Console.WriteLine($"Third Response : {rateLimiter.ShouldThrottleRequest("UserId$1234:", 1)}");
            Console.WriteLine($"new user Response : {rateLimiter.ShouldThrottleRequest("UserId$1236:", 1)}");
            Console.WriteLine($"new ip user Response : {rateLimiter.ShouldThrottleRequest("UserId$Ip$:124:123", 1)}");
            Console.WriteLine($"new ip user Response : {rateLimiter.ShouldThrottleRequest("UserId$Ip$:124:123", 18)}");
            Console.WriteLine($"new ip user Response : {rateLimiter.ShouldThrottleRequest("UserId$Ip$:124:123", 3)}");
            Console.WriteLine($"new ip user Response11 : {rateLimiter.ShouldThrottleRequest("UserId$Ip$:125:123", 3)}");
            Thread.Sleep(30000);
            Console.WriteLine($"Fourth Response : {rateLimiter.ShouldThrottleRequest("UserId$1234:", 1)}");
            Console.WriteLine($"Fifth Response : {rateLimiter.ShouldThrottleRequest("UserId$1234:", 10)}");
            Console.WriteLine($"6 Response : {rateLimiter.ShouldThrottleRequest("UserId$1234:", 9)}");
            Console.WriteLine($"7 Response : {rateLimiter.ShouldThrottleRequest("UserId$1234:", 1)}");
            // app.Run();
        }
    }
}