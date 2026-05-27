using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sicoain.shared.DTOs
{
    public abstract record EntityEmailDto
    {
        public int Id { get; init; }
        public required string Email { get; init; }
    }
}
