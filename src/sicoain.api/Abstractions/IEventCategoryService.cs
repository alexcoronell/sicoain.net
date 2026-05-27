using sicoain.shared.DTOs.EventCategories;

namespace sicoain.api.Abstractions
{
    public interface IEventCategoryService : IBaseService<EventCategoryDto, CreateEventCategoryRequest, UpdateEventCategoryRequest>
    {

    }
}
