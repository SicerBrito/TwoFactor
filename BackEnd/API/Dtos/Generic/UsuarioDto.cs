using API.Dtos.Cita;
using API.Dtos.Venta;

namespace API.Dtos;
    public class UsuarioDto{
        public string Usename { get; set; } = null!;    
        public string Email { get; set; } = null!;

        public List<CitaComplementsDto> ? Citas { get; set; }
        public List<VentaComplementsDto> ? Ventas { get; set; }
    }
