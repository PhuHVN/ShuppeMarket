using AutoMapper;
using ShuppeMarket.Application.DTOs.AccountDtos;
using ShuppeMarket.Application.DTOs.ProductDtos;
using ShuppeMarket.Application.DTOs.SellerDtos;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Domain.Entities;

namespace MuseumSystem.Application.Dtos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Config Mapper Paging
            CreateMap(typeof(BasePaginatedList<>), typeof(BasePaginatedList<>))
                .ConvertUsing(typeof(BasePaginatedListConverter<,>));
            //Other Mappings
            CreateMap<Account, AccountResponse>();
            CreateMap<Seller, SellerResponse>()
               .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.Account.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Account.FullName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Account.Address))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Account.Role));

            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.CategoryNames, opt => opt.MapFrom(src => src.CategoryProducts.Select(cp => cp.Category.Name).ToList()));

        }
        public class BasePaginatedListConverter<TSource, TDestination> : ITypeConverter<BasePaginatedList<TSource>, BasePaginatedList<TDestination>>
        {
            public BasePaginatedList<TDestination> Convert(
                BasePaginatedList<TSource> source,
                BasePaginatedList<TDestination> destination,
                ResolutionContext context)
            {
                var mappedItems = context.Mapper.Map<List<TDestination>>(source.Items);

                return new BasePaginatedList<TDestination>(
                    mappedItems,
                    source.TotalItems,
                    source.PageIndex,
                    source.PageSize
                );
            }
        }
    }
}
