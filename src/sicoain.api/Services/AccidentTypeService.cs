using AutoMapper;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs.AccidentTypes;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class AccidentTypeService : BaseService<AccidentType, AccidentTypeDto, CreateAccidentTypeRequest, UpdateAccidentTypeRequest>, IAccidentTypeService
    {
        public AccidentTypeService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
