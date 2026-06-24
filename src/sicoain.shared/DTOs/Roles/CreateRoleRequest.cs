using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sicoain.shared.DTOs.Roles
{
    public record CreateRoleRequest
    {
        [Required]
        [MinLength(5)]
        public string Name { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public bool IsActive { get; init; }
    }
}
