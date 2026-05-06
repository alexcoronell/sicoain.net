using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Enums
{
    public enum Priority
    {
        [Display(Name = "Baja")]
        Low = 0,
        [Display(Name = "Media")]
        Medium = 1,
        [Display(Name = "Alta")]
        High = 2,
        [Display(Name = "Crítica")]
        Critical = 3
    }
}
