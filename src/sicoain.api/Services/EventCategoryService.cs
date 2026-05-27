using AutoMapper;
using sicoain.api.Abstractions;
using sicoain.api.Data;
using sicoain.shared.DTOs.EventCategories;
using sicoain.shared.Entities;

namespace sicoain.api.Services
{
    public class EventCategoryService : BaseService<EventCategory, EventCategoryDto, CreateEventCategoryRequest, UpdateEventCategoryRequest>, IEventCategoryService
    {
        public EventCategoryService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
