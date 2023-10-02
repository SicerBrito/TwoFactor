using API.Dtos;
using Dominio.Entities;

namespace API.Services;
    public interface IUserService{

        byte[] CreateQR(ref Usuario usuario);
        bool VerifyCode(string secret, string code);
        Task<string> RegisterAsync(RegisterDto model);
        Task<DataUserDto> GetTokenAsync(LoginDto model);
        Task<string> AddRoleAsync(AddRoleDto model);
        Task<DataUserDto> RefreshTokenAsync(string refreshToken);
        
    }
