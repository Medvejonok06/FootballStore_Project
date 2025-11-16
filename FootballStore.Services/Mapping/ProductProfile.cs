using AutoMapper;
using FootballStore.Data.Ef.Entities;
using FootballStore.Services.DTOs;

namespace FootballStore.Services.Mapping
{
    // DTO + AutoMapper профілі (2.00 балів)
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // Мапінг Entity -> Read DTO
            CreateMap<Product, ProductDto>(); 

            // Мапінг Create DTO -> Entity (для створення)
            CreateMap<ProductCreateDto, Product>();
        }
    }
}