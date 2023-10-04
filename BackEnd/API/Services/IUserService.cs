using API.Dtos;
using Dominio.Entities;

namespace API.Services;
    public interface IUserService{

        byte[] CreateQR(ref Usuario usuario);
        bool VerifyCode(string secret, string code);
        
    }
