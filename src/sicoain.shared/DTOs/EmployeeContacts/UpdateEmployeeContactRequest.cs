using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sicoain.shared.DTOs.EmployeeContacts
{
    public class UpdateEmployeeContactRequest
    {
        public string? Fullname { get; init; }
        public string? Relationship { get; init; }
    }
}
