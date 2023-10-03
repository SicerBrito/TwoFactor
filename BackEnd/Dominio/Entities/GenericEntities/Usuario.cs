
namespace Dominio.Entities;
    public class Usuario : BaseEntity{
        
        public string ? Username { get; set; }
        public string ? Email { get; set; }
        public string ? Password { get; set; }
        public string ? TwoFactorSecret { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public ICollection<Rol> ? Roles { get; set; } = new HashSet<Rol>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
        public ICollection<UsuarioRol> ? UsuarioRoles { get; set; }
}
