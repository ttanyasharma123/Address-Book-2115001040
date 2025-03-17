using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace BusinessLayer.Service
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly StackExchange.Redis.IDatabase _cache;
        private readonly int _cacheDuration;

        public RedisCacheService(IConfiguration configuration)
        {
            try
            {
                var redis = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]);
                _cache = redis.GetDatabase();
                _cacheDuration = int.Parse(configuration["Redis:CacheDuration"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Redis connection failed: {ex.Message}");
            }
        }
    }

}
