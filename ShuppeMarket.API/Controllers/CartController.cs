using Microsoft.AspNetCore.Mvc;
using ShuppeMarket.Application.DTOs;
using ShuppeMarket.Application.DTOs.Cart;
using ShuppeMarket.Application.Interfaces;

namespace ShuppeMarket.API.Controllers
{
    [Route("api/v1/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartRequest request)
        {
            await _cartService.AddToCartAsync(request);
            return Ok(ApiResponse<string>.CreateResponse("Item added to cart successfully"));
        }
        [HttpDelete("remove/{detailId}")]
        public async Task<IActionResult> RemoveFromCart(string detailId)
        {
            await _cartService.RemoveFromCartAsync(detailId);
            return NoContent();
        }
        [HttpPut("update/{detailId}/quantity/{quantity}")]
        public async Task<IActionResult> UpdateCartItem(string detailId, int quantity)
        {
            await _cartService.UpdateCartItemAsync(detailId, quantity);
            return Ok(ApiResponse<string>.CreateResponse("Cart item updated successfully"));
        }
        [HttpGet("my-cart")]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _cartService.GetCartAsyncByUserLogin();
            return Ok(ApiResponse<CartResponse>.OkResponse(cart.Value, "Get cart successfully", "200"));
        }
    }
}
