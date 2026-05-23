using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sicoain.shared.DTOs.HealthPromotionEntities
{
    public class UpdateHealthPromotionEntityRequest
    {
        public string? Name { get; init; }
        public string? AddressStreet { get; init; }
        public string? Notes { get; init; }
    }
}
