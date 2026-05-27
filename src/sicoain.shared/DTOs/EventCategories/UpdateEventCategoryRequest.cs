using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sicoain.shared.Enums;

namespace sicoain.shared.DTOs.EventCategories
{
    public class UpdateEventCategoryRequest
    {
        public string? Name { get; init; }
        public AccidentSeverity? LevelOfSeverity { get; init; }
        public bool RequiresHospitalization { get; init; }
    }
}
