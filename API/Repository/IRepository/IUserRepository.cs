using API.Models;
using API.DTOs;

namespace API.Repository.IRepository
{
    public interface IUserRespository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO);
    }
}
