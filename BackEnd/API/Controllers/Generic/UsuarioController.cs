using API.Dtos;
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

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync(RegisterDto model)
    {
        var result = await _UserService.RegisterAsync(model);
        return Ok(result);
    }


    [HttpPost("token")]
    public async Task<IActionResult> GetTokenAsync(LoginDto model)
    {
        var result = await _UserService.GetTokenAsync(model);
        return Ok(result);
    }

    [HttpPost("addrole")]
    public async Task<IActionResult> AddRoleAsync(AddRoleDto model)
    {
        var result = await _UserService.AddRoleAsync(model);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = await _UserService.RefreshTokenAsync(refreshToken!);
        if (!string.IsNullOrEmpty(response.RefreshToken))
            SetRefreshTokenInCookie(response.RefreshToken);
        return Ok(response);
    }


    private void SetRefreshTokenInCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(10),
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
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
            return BadRequest("some wrong");
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
                return Ok("authenticated!!");
            }

            return Unauthorized();
        }
        catch (Exception ex){
            _Logger.LogError(ex.Message);
            return BadRequest("some wrong");
        }  
                               
    }


}