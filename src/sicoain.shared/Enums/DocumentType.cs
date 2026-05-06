using System.ComponentModel.DataAnnotations;

namespace sicoain.shared.Enums
{
    public enum DocumentType
    {
        [Display(Name = "Tarjeta de Identidad")]
        TarjetaDeIdentidad = 1,

        [Display(Name = "Cédula de Ciudadanía")]
        CedulaDeCiudadania = 2,

        [Display(Name = "Cédula de Extranjería")]
        CedulaDeExtranjeria = 3,

        [Display(Name = "Pasaporte")]
        Pasaporte = 4,

        [Display(Name = "NIT -Número de Identificación Tributaria")]
        NumeroDeIdentificacionTributaria = 5,

        [Display(Name = "Permiso Especial de Permanencia")]
        PermisoEspecialDePermanencia = 6
    }
}
