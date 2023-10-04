using Dominio.Entities;
using TwoFactorAuthNet;
using TwoFactorAuthNet.Providers.Qr;

namespace API.Services;
public class UserService : IUserService{
        private readonly IConfiguration _Conf;
        private readonly int _AccessTokenDuration;
        private readonly int _RefreshTokenTokenDuration;
        public UserService(
            IConfiguration conf 
            )
        {
            _Conf = conf;
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

}


