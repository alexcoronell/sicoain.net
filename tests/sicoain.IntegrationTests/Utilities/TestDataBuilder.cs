using sicoain.shared.DTOs.Accident;
using sicoain.shared.DTOs.Employees;
using sicoain.shared.Enums;

namespace sicoain.IntegrationTests.Utilities;

public static class TestDataBuilder
{
    public static CreateAccidentRequest CreateValidAccidentRequest(int employeeId, int accidentTypeId, int eventCategoryId)
    {
        return new CreateAccidentRequest
        {
            EventDate = DateTime.UtcNow,
            Description = "Prueba de accidente - Descripción generada automáticamente",
            EmployeeId = employeeId,
            AccidentTypeId = accidentTypeId,
            EventCategoryId = eventCategoryId
        };
    }

    public static CreateEmployeeRequest CreateValidEmployeeRequest(
        int businessId,
        int branchId,
        int positionId,
        int departmentId,
        int healthPromotionEntityId,
        int occupationalRiskAdministratorId)
    {
        return new CreateEmployeeRequest
        {
            DocumentType = DocumentType.CedulaDeCiudadania,
            DocumentNumber = "1234567890",
            FirstName = "Juan",
            SecondName = "Carlos",
            Surname = "Pérez",
            SecondSurname = "González",
            State = "Cundinamarca",
            Municipality = "Bogotá",
            Neighborhood = "Chapinero",
            AddressStreet = "Carrera 7 # 71 - 21",
            HiringDate = DateTime.UtcNow.AddDays(-30),
            BusinessId = businessId,
            BranchId = branchId,
            PositionId = positionId,
            DepartmentId = departmentId,
            HealthPromotionEntityId = healthPromotionEntityId,
            OccupationalRiskAdministratorId = occupationalRiskAdministratorId
        };
    }
}
