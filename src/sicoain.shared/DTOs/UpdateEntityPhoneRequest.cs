using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sicoain.shared.DTOs
{
    public record UpdateEntityPhoneRequest
    {
        [Required, MaxLength(20)]
        public required string Phone { get; init; }
    }
}
