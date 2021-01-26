using System.ComponentModel.DataAnnotations;

namespace pixit.Shared.Models
{
    public class CreateRoomModel
    {
        [Display(Name = "Nazwa pokoju")]
        [Required(ErrorMessage = "{0} jest wymagana.")]
        [StringLength(32, ErrorMessage = "{0} musi zawierać od {2} do {1} znaków.", MinimumLength = 3)]
        public string Name { get; set; }
        
        [Display(Name = "Liczba graczy")]
        [Range(2, 20, ErrorMessage = "{0} musi zawierać się pomiędzy {1} i {2}." )]
        public int Slots { get; set; } = 5;
        
        [Display(Name = "Liczba punktów")]
        [Range(5, 100, ErrorMessage = "{0} musi zawierać się pomiędzy {1} i {2}." )]
        public int MaxScore { get; set; } = 10;
    }
}