using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sicoain.shared.DTOs.Deparments
{
    public record UpdateDepartmentRequest
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public string? Email { get; init; }
        public string? Phone { get; init; }
    }
}
