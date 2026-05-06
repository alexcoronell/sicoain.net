using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Enums
{
    public enum AttachmentEntityType
    {
        [Display(Name = "Accidente/Incidente")]
        Accident = 1,

        [Display(Name = "Acción correctiva")]
        CorrectiveAction = 2,

        [Display(Name = "Testigo")]
        Witness = 3,

        [Display(Name = "Empleado")]
        Employee = 4
    }
}
