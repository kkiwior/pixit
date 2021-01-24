using System.ComponentModel.DataAnnotations;

namespace pixit.Server.Models
{
    public class UserModel
    {
        [Display(Name = "Nazwa użytkownika")]
        [Required(ErrorMessage = "{0} jest wymagana.")]
        [StringLength(32, ErrorMessage = "{0} musi zawierać od {2} do {1} znaków.", MinimumLength = 3)]
        public string Name { get; set; }
    }
}