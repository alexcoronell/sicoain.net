using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Enums
{
    public enum AccidentSeverity
    {
        [Display(Name = "Incidente (Sin Lesión)")]
        Incident = 0,

        [Display(Name = "Leve")]
        Mild = 1,

        [Display(Name = "Moderado")]
        Moderate = 2,

        [Display(Name = "Grave")]
        Severe = 3,


        [Display(Name = "Muy Grave / Mortal")]
        Critico = 4
    }
}
