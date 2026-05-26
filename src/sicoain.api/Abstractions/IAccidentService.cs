using sicoain.shared.DTOs.Accident;

namespace sicoain.api.Abstractions
{
    public interface IAccidentService : IBaseService<AccidentDto, CreateAccidentRequest, UpdateAccidentRequest>
    {

    }
}
