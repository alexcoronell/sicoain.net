using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sicoain.shared.DTOs
{
    public abstract record EntityPhoneDto
    {
        public int Id { get; init; }
        public required string PhoneNumber { get; init; }
    }
}
