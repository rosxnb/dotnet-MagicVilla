using AutoMapper;
using API.Models;
using API.DTOs;

namespace API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            _ = CreateMap<Villa, VillaDTO>().ReverseMap();
            _ = CreateMap<Villa, VillaCreateDTO>().ReverseMap();
            _ = CreateMap<Villa, VillaUpdateDTO>().ReverseMap();
        }
    }
}
