using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Enums
{
    public enum PhoneType
    {
        [Display(Name = "Celular")]
        Mobile = 0,
        [Display(Name = "Casa")]
        Home = 1,
        [Display(Name = "Trabajo")]
        Work = 2,
        [Display(Name = "Otro")]
        Other = 3,
    }
}
