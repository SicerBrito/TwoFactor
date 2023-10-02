using System.ComponentModel.DataAnnotations;

namespace API.Dtos.Generic;
    public class AuthVerifyCodeDto
    {
        [Required]
        public string Code { get; set; } = String.Empty;

        [Required]
        public long Id { get; set; }
    }
