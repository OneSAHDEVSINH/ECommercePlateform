using AutoMapper;
using ECommercePlateform.Server.Core.Application.DTOs;
using ECommercePlateform.Server.Core.Domain.Entities;

namespace ECommercePlateform.Server.Core.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Country mappings
            CreateMap<Country, CountryDto>()
                .ForMember(dest => dest.States, opt => opt.MapFrom(src => src.States));
            CreateMap<CreateCountryDto, Country>();
            CreateMap<UpdateCountryDto, Country>();

            // State mappings
            CreateMap<State, StateDto>()
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name))
                .ForMember(dest => dest.Cities, opt => opt.MapFrom(src => src.Cities));
            CreateMap<CreateStateDto, State>();
            CreateMap<UpdateStateDto, State>();

            // City mappings
            CreateMap<City, CityDto>()
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.State.Name))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.State.CountryId))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.State.Country.Name));
            CreateMap<CreateCityDto, City>();
            CreateMap<UpdateCityDto, City>();

            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants));
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // ProductVariant mappings
            CreateMap<ProductVariant, ProductVariantDto>();

            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
        }
    }
} 