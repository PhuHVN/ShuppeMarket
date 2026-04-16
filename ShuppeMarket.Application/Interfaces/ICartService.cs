using ShuppeMarket.Application.DTOs.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Interfaces
{
    public interface ICartService
    {
            Task AddToCartAsync(CartRequest request);
            Task RemoveFromCartAsync(string detailId);
            Task UpdateCartItemAsync(string detailId, int quantity);
            Task<CartResponse> GetCartAsyncByUserLogin();
    }
}
