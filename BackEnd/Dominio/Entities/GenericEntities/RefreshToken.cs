namespace Dominio.Entities;
    public class RefreshToken : BaseEntity{
        
        public int UserId { get; set; }
        public Usuario ? User { get; set; }
        public string ? Token { get; set; }
        public DateTime Expires { get; set; } = DateTime.UtcNow;
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Revoked { get; set; } = DateTime.UtcNow;
        public bool IsActive => Revoked == null && !IsExpired;
        
    }
