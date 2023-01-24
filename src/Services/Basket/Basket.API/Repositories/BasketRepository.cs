using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redis;
        ILogger<BasketRepository> _logger;
        public BasketRepository(IDistributedCache redis, ILogger<BasketRepository> logger)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task DeleteBasket(string username)
        {
            await _redis.RemoveAsync(username);
        }

        public async Task<ShoppingCart?> GetBasket(string username)
        {
            var basket = await _redis.GetStringAsync(username);
            if (string.IsNullOrEmpty(basket)) return null;
            return JsonConvert.DeserializeObject<ShoppingCart?>(basket);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await _redis.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));
            return await GetBasket(basket.UserName);
        }
    }
}
