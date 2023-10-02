namespace Dominio.Entities;
    public class UsuarioRol : BaseEntity{
        
        public int UsuarioId { get; set; }
        public Usuario ? Usuarios { get; set; }
        public int RolId { get; set; }
        public Rol ? Roles { get; set; }
        
    }
