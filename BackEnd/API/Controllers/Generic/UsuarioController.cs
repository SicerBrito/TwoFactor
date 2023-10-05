using API.Dtos.Generic;
using API.Services;
using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class UsuarioController : BaseApiController
{
    private readonly ILogger<UsuarioController> _Logger;
    private readonly IUnitOfWork _UnitOfWork;
    private readonly IUserService _UserService;

    public UsuarioController(
        ILogger<UsuarioController> logger,
        IUnitOfWork unitOfWork,
        IUserService userService)
    {
        _Logger = logger;
        _UnitOfWork = unitOfWork;
        _UserService = userService;
    }

    [HttpGet("QR/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]    
    public async Task<ActionResult> GetQR(long id){        
        
        try{
            Usuario usuario = await _UnitOfWork.Usuarios!.FindFirst(x => x.Id == id);
            byte[] QR = _UserService.CreateQR(ref usuario);            

            _UnitOfWork.Usuarios.Update(usuario);
            await _UnitOfWork.SaveAsync();
            return File(QR,"image/png");
        }
        catch (Exception ex){
            _Logger.LogError(ex.Message);
            return BadRequest("The QR code could not be generated");
        }  
                               
    }

    [HttpGet("Verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]    
    [ProducesResponseType(StatusCodes.Status400BadRequest)]    
    public async Task<ActionResult> Verify([FromBody] AuthVerifyCodeDto data){        
        try{

            Usuario usuario = await _UnitOfWork.Usuarios!.FindFirst(x => x.Id == data.Id);
            if(usuario.TwoFactorSecret == null){
                throw new ArgumentNullException(usuario.TwoFactorSecret);
            }
            var isVerified = _UserService.VerifyCode(usuario.TwoFactorSecret, data.Code);            

            if(isVerified == true){
                // Descargar una imagen desde una URL remota en caso de autenticación exitosa
                using (var httpClient = new HttpClient())
                {
                    byte[] imageBytes2 = await httpClient.GetByteArrayAsync("https://http.cat/200");

                    // Especificar el tipo de contenido de la respuesta como "image/png"
                    Response.ContentType = "image/png";

                    // Devolver los datos de imagen como un arreglo de bytes
                    return File(imageBytes2, "image/png");
                }
            }

            // Cargar una imagen en bytes (por ejemplo, una imagen PNG)
            byte[] imageBytes = System.IO.File.ReadAllBytes("C:/Users/APM01-53/Documents/TwoFactor/BackEnd/Img/401.jpg");

            // Devolver la imagen como respuesta con el tipo de contenido "image/png"
            return File(imageBytes, "image/png");

        }
        catch (Exception ex){
            _Logger.LogError(ex.Message);
            // En caso de excepción, devolver una imagen como respuesta
            // Puedes cargar una imagen diferente aquí si lo deseas
            byte[] errorImageBytes = System.IO.File.ReadAllBytes("C:/Users/APM01-53/Documents/TwoFactor/BackEnd/Img/400.jpg");

            return File(errorImageBytes, "image/png");
        }  
                               
    }
}