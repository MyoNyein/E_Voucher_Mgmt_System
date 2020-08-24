using AutoMapper;
using E_Voucher.Entities.Response_Models;

namespace E_Voucher.Entities.DTO.MapConfiguration
{
    public class DTOMappingProfile : Profile
    {
        public DTOMappingProfile()
        {
            CreateMap<LoginResponseDTO, LoginResponse>();
        }
        
    }
}
