using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Dtos;
using API.Helpers;
using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TwoFactorAuthNet;
using TwoFactorAuthNet.Providers.Qr;


namespace API.Services;
public class UserService : IUserService{
        private readonly JWT _Jwt;
        private readonly IConfiguration _Conf;
        private readonly int _AccessTokenDuration;
        private readonly int _RefreshTokenTokenDuration;
        private readonly ILogger<UserService> _Logger;
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IPasswordHasher<Usuario> _PasswordHasher;
        public UserService(
            IUnitOfWork unitOfWork,
            IOptions<JWT> jwt,
            IPasswordHasher<Usuario> passwordHasher,
            IConfiguration conf ,
            ILogger<UserService> logger
            )
        {
            _Jwt = jwt.Value;
            _UnitOfWork = unitOfWork;
            _PasswordHasher = passwordHasher;
            _Conf = conf;
            _Logger = logger;
            //--Token duration
            _ = int.TryParse(conf["JWTSettings:AccessTokenTimeInMinutes"], out _AccessTokenDuration);
            _ = int.TryParse(conf["JWTSettings:RefreshTokenTimeInHours"], out _RefreshTokenTokenDuration);   
        }

        

        public byte[] CreateQR(ref Usuario usuario){        
            if( usuario.Email == null){
                throw new ArgumentNullException(usuario.Email);
            }        
            var tfa = new TwoFactorAuth(_Conf["JWTSettings:Issuer"],6,30,Algorithm.SHA256, new ImageChartsQrCodeProvider());
            string secret = tfa.CreateSecret(160);
            usuario.TwoFactorSecret = secret;

            var QR = tfa.GetQrCodeImageAsDataUri(usuario.Email, usuario.TwoFactorSecret); 

            string UriQR = QR.Replace("data:image/png;base64,", "");


            return Convert.FromBase64String(UriQR);        
        }

        public bool VerifyCode(string secret, string code){        
            var tfa = new TwoFactorAuth(_Conf["JWTSettings:Issuer"],6,30,Algorithm.SHA256);
            return tfa.VerifyCode(secret, code);
        }






        private RefreshToken CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    Expires = DateTime.UtcNow.AddDays(10),
                    Created = DateTime.UtcNow
                };
            }
        }
        async Task<string> IUserService.RegisterAsync(RegisterDto registerDto)
        {
            var usuario = new Usuario
            {
                Email = registerDto.Email,
                Username = registerDto.Username,

            };

            usuario.Password = _PasswordHasher.HashPassword(usuario, registerDto.Password!);

            var usuarioExiste = _UnitOfWork.Usuarios!
                                                .Find(u => u.Username!.ToLower() == registerDto.Username!.ToLower())
                                                .FirstOrDefault();

            if (usuarioExiste == null)
            {
                var rolPredeterminado = _UnitOfWork.Roles!
                                                     .Find(u => u.Nombre == Autorizacion.rol_predeterminado.ToString())
                                                     .First();
                try
                {
                    usuario.Roles!.Add(rolPredeterminado);
                    _UnitOfWork.Usuarios.Add(usuario);
                    await _UnitOfWork.SaveAsync();

                    return $"El Usuario {registerDto.Username} ha sido registrado exitosamente";
                }

                catch (Exception ex)
                {
                    var message = ex.Message;
                    return $"Error: {message}";
                }
            }
            else
            {

                return $"El usuario con {registerDto.Username} ya se encuentra resgistrado.";
            }

        }

        async Task<string> IUserService.AddRoleAsync(AddRoleDto model)
        {
            var usuario = await _UnitOfWork.Usuarios!
                                .GetByUsernameAsync(model.Username!);

            if (usuario == null)
            {
                return $"No existe algun usuario registrado con la cuenta olvido algun caracter?{model.Username}.";
            }

            var resultado = _PasswordHasher.VerifyHashedPassword(usuario, usuario.Password!, model.Password!);

            if (resultado == PasswordVerificationResult.Success)
            {
                var rolExiste = _UnitOfWork.Roles!
                                                .Find(u => u.Nombre!.ToLower() == model.Rol!.ToLower())
                                                .FirstOrDefault();

                if (rolExiste != null)
                {
                    var usuarioTieneRol = usuario.Roles!
                                                    .Any(u => u.Id == rolExiste.Id);

                    if (usuarioTieneRol == false)
                    {
                        usuario.Roles!.Add(rolExiste);
                        _UnitOfWork.Usuarios.Update(usuario);
                        await _UnitOfWork.SaveAsync();
                    }

                    return $"Rol {model.Rol} agregado a la cuenta {model.Username} de forma exitosa.";
                }

                return $"Rol {model.Rol} no encontrado.";
            }

            return $"Credenciales incorrectas para el ususario {usuario.Username}.";
        }
        public async Task<DataUserDto> GetTokenAsync(LoginDto model)
        {
            DataUserDto datosUsuarioDto = new DataUserDto();
            var usuario = await _UnitOfWork.Usuarios!
                            .GetByUsernameAsync(model.Username!);

            if (usuario == null)
            {
                datosUsuarioDto.IsAuthenticated = false;
                datosUsuarioDto.Message = $"No existe ningun usuario con el username {model.Username}.";
                return datosUsuarioDto;
            }

            var result = _PasswordHasher.VerifyHashedPassword(usuario, usuario.Password!, model.Password!);
            if (result == PasswordVerificationResult.Success)
            {
                datosUsuarioDto.IsAuthenticated = true;
                datosUsuarioDto.Message = "OK";
                datosUsuarioDto.IsAuthenticated = true;
                if (usuario != null && usuario != null)
                {
                    JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);
                    datosUsuarioDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                    datosUsuarioDto.UserName = usuario.Username;
                    datosUsuarioDto.Email = usuario.Email;
                    datosUsuarioDto.Roles = (usuario.Roles!
                                                        .Select(p => p.Nombre)
                                                        .ToList())!;


                     if (usuario.RefreshTokens.Any(a => a.IsActive))
                        {
                            var activeRefreshToken = usuario.RefreshTokens.Where(a => a.IsActive == true).FirstOrDefault();
                            datosUsuarioDto.RefreshToken = activeRefreshToken!.Token;
                            datosUsuarioDto.RefreshTokenExpiration = activeRefreshToken.Expires;
                        }
                        else
                        {
                            var refreshToken = CreateRefreshToken();
                            datosUsuarioDto.RefreshToken = refreshToken.Token;
                            datosUsuarioDto.RefreshTokenExpiration = refreshToken.Expires;
                            usuario.RefreshTokens.Add(refreshToken);
                            _UnitOfWork.Usuarios.Update(usuario);
                            await _UnitOfWork.SaveAsync();
                        }

                        return datosUsuarioDto;
                }
                else{

                    datosUsuarioDto.IsAuthenticated = false;
                    datosUsuarioDto.Message = $"Credenciales incorrectas para el usuario {usuario!.Username}.";

                    return datosUsuarioDto;
                }
            }
            
            // Valor de retorno predeterminado en caso de que ninguna condición se cumpla
            return datosUsuarioDto;

        }


        private JwtSecurityToken CreateJwtToken(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario), "El usuario no puede ser nulo.");
            }

            var roles = usuario.Roles;
            var roleClaims = new List<Claim>();
            foreach (var rol in roles!)
            {
                roleClaims.Add(new Claim("roles", rol.Nombre!));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Username!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", usuario.Id.ToString())
            }
            .Union(roleClaims);

            if (string.IsNullOrEmpty(_Jwt.Key) || string.IsNullOrEmpty(_Jwt.Issuer) || string.IsNullOrEmpty(_Jwt.Audience))
            {
                throw new ArgumentNullException("La configuración del JWT es nula o vacía.");
            }

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Jwt.Key));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var JwtSecurityToken = new JwtSecurityToken(
                issuer: _Jwt.Issuer,
                audience: _Jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_Jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return JwtSecurityToken;
        }

    async Task<DataUserDto> IUserService.RefreshTokenAsync(string refreshToken)
    {
        var datosUsuarioDto = new DataUserDto();

        var usuario = await _UnitOfWork.Usuarios!
                        .GetByRefreshTokenAsync(refreshToken);

        if (usuario == null)
        {
            datosUsuarioDto.IsAuthenticated = false;
            datosUsuarioDto.Message = $"Token is not assigned to any user.";
            return datosUsuarioDto;
        }

        var refreshTokenBd = usuario.RefreshTokens.Single(x => x.Token == refreshToken);

        if (!refreshTokenBd.IsActive)
        {
            datosUsuarioDto.IsAuthenticated = false;
            datosUsuarioDto.Message = $"Token is not active.";
            return datosUsuarioDto;
        }
        //Revoque the current refresh token and
        refreshTokenBd.Revoked = DateTime.UtcNow;
        //generate a new refresh token and save it in the database
        var newRefreshToken = CreateRefreshToken();
        usuario.RefreshTokens.Add(newRefreshToken);
        _UnitOfWork.Usuarios.Update(usuario);
        await _UnitOfWork.SaveAsync();
        //Generate a new Json Web Token
        datosUsuarioDto.IsAuthenticated = true;
        JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);
        datosUsuarioDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        datosUsuarioDto.Email = usuario.Email;
        datosUsuarioDto.UserName = usuario.Username;
        datosUsuarioDto.Roles = (usuario.Roles!
                                        .Select(u => u.Nombre)
                                        .ToList())!;
        datosUsuarioDto.RefreshToken = newRefreshToken.Token;
        datosUsuarioDto.RefreshTokenExpiration = newRefreshToken.Expires;
        return datosUsuarioDto;
    }

}





