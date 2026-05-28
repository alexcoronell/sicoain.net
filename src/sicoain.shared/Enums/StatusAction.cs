using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Enums
{
    public enum StatusAction
    {
        [Display(Name = "Rechazada")]
        Rejected = 0,

        [Display(Name = "Propuesta")]
        Proposal = 1,

        [Display(Name = "Aprobada")]
        Approved = 2,

        [Display(Name = "En proceso")]
        InProcess = 3,

        [Display(Name = "Completada")]
        Completed = 4,
    }
}
