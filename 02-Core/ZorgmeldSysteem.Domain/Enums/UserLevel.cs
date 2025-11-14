using System.ComponentModel.DataAnnotations;

namespace ZorgmeldSysteem.Domain.Enums
{
    public enum UserLevel
    {
        [Display(Name = "Fixility Admin")]
        Admin = 1,

        [Display(Name = "Bedrijfsbeheerder")]
        Manager = 2,

        [Display(Name = "Monteur")]
        Mechanic = 3,

        [Display(Name = "Melder")]
        Reporter = 4
    }
}