using AutoMapper;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs.Business;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class BusinessService : BaseService<Business, BusinessDto, CreateBusinessRequest, UpdateBusinessRequest>, IBusinessService
    {
        public BusinessService(ApplicationDbContext context, IMapper _mapper) : base(context, _mapper)
        {
        }
    }
}
