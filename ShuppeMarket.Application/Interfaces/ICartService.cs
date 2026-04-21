using ShuppeMarket.Application.DTOs.Cart;
using ShuppeMarket.Domain.ResultError;

namespace ShuppeMarket.Application.Interfaces
{
    public interface ICartService
    {
        Task<Result<string>> AddToCartAsync(CartRequest request);
        Task<Result<string>> RemoveFromCartAsync(string detailId);
        Task<Result<string>> UpdateCartItemAsync(string detailId, int quantity);
        Task<Result<CartResponse>> GetCartAsyncByUserLogin();
    }
}
