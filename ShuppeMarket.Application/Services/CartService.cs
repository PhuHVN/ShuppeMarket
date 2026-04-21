using Microsoft.EntityFrameworkCore;
using ShuppeMarket.Application.DTOs.Cart;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;
using ShuppeMarket.Domain.ResultError;

namespace ShuppeMarket.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;

        public CartService(IUnitOfWork unitOfWork, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
        }
        public async Task<Result<string>> AddToCartAsync(CartRequest request)
        {
            var user = await _authService.GetCurrentUserLoginAsync();

            if (user.IsFailure || user.Value == null)
            {
                return Result<string>.Fail("UNAUTHORIZED", "User not authenticated");
            }

            if (request.Products == null || !request.Products.Any())
            {
                return Result<string>.Fail("INVALID_REQUEST", "No products provided to add to cart");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Get or create cart
                var cart = await _unitOfWork.GetRepository<Cart>().FindAsync(x => x.AccountId == user.Value.Id && x.IsActive);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        AccountId = user.Value.Id,
                        IsActive = true,
                        CreateAt = DateTime.UtcNow
                    };
                    await _unitOfWork.GetRepository<Cart>().InsertAsync(cart);
                    await _unitOfWork.SaveChangesAsync();
                }

                // Validate products exist
                var productIds = request.Products.Select(p => p.ProductId).ToList();
                var products = await _unitOfWork.GetRepository<Product>()
                    .GetQueryable()
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                if (!products.Any())
                {
                    throw new Exception($"No products found with IDs: {string.Join(", ", productIds)}");
                }

                var validProductIds = products.Select(p => p.Id).ToHashSet();

                // Process each product
                foreach (var requestProduct in request.Products.Where(p => validProductIds.Contains(p.ProductId)))
                {
                    // Check if CartDetail exists in database
                    var existingDetail = await _unitOfWork.GetRepository<CartDetail>()
                        .FindAsync(x => x.CartId == cart.Id && x.ProductId == requestProduct.ProductId);

                    if (existingDetail != null)
                    {
                        // Update existing CartDetail
                        existingDetail.Quantity += requestProduct.Quantity;
                        await _unitOfWork.GetRepository<CartDetail>().UpdateAsync(existingDetail);
                    }
                    else
                    {
                        // Insert new CartDetail
                        var newCartDetail = new CartDetail
                        {
                            CartId = cart.Id,
                            ProductId = requestProduct.ProductId,
                            Quantity = requestProduct.Quantity,
                            CreateAt = DateTime.UtcNow,
                            PriceAtTime = products.First(p => p.Id == requestProduct.ProductId).Price
                        };
                        await _unitOfWork.GetRepository<CartDetail>().InsertAsync(newCartDetail);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return Result<string>.Success("Products added to cart successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                throw new Exception("Failed to add item to cart", ex);
            }
        }

        public async Task<Result<CartResponse>> GetCartAsyncByUserLogin()
        {
            var user = await _authService.GetCurrentUserLoginAsync();
            if (user.IsFailure || user.Value == null)
            {
                return Result<CartResponse>.Fail("UNAUTHORIZED", "User not authenticated");
            }
            var cart = await _unitOfWork.GetRepository<Cart>()
                .FindAsync(x => x.AccountId == user.Value.Id && x.IsActive, include: x => x.Include(x => x.Account).Include(x => x.CartDetails).ThenInclude(cd => cd.Product).ThenInclude(x => x.Seller));
            if (cart == null)
            {
                return Result<CartResponse>.Fail("NOT_FOUND", "Active cart not found for user");
            }
            var cartDetails = cart.CartDetails.Select(cd => new CartDetailResponse
            {
                CartDetailId = cd.Id,
                ProductId = cd.ProductId,
                ProductImage = cd.Product.ImageUrl,
                ProductName = cd.Product.Name,
                ShopId = cd.Product.SellerId,
                ShopName = cd.Product.Seller.ShopName,
                Quantity = cd.Quantity,
                Price = cd.Product.Price
            }).ToList();

            var cartResponse = new CartResponse
            {
                CartId = cart.Id,
                CartDetails = cartDetails
            };
            return Result<CartResponse>.Success(cartResponse);
        }

        public async Task<Result<string>> RemoveFromCartAsync(string detailId)
        {
            var user = await _authService.GetCurrentUserLoginAsync();
            if (user.IsFailure || user.Value == null)
            {
                return Result<string>.Fail("UNAUTHORIZED", "User not authenticated");
            }
            var cartDetail = await _unitOfWork.GetRepository<CartDetail>().FindAsync(x => x.Id == detailId);
            if (cartDetail == null)
            {
                return Result<string>.Fail("NOT_FOUND", "Cart detail not found");
            }
            await _unitOfWork.GetRepository<CartDetail>().DeleteAsync(cartDetail);
            await _unitOfWork.SaveChangesAsync();
            return Result<string>.Success("Cart detail removed successfully");
        }

        public async Task<Result<string>> UpdateCartItemAsync(string detailId, int quantity)
        {
            var cartDetail = await _unitOfWork.GetRepository<CartDetail>().FindAsync(x => x.Id == detailId);
            if (cartDetail == null)
            {
                return Result<string>.Fail("NOT_FOUND", "Cart detail not found");
            }
            if (quantity <= 0)
            {
                return Result<string>.Fail("INVALID", "Quantity must be greater than zero");
            }
            cartDetail.Quantity = quantity;
            await _unitOfWork.GetRepository<CartDetail>().UpdateAsync(cartDetail);
            await _unitOfWork.SaveChangesAsync();
            return Result<string>.Success("Cart detail updated successfully");
        }
    }
}
