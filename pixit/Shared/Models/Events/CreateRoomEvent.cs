using System.ComponentModel.DataAnnotations;

namespace pixit.Shared.Models.Events
{
    public class CreateRoomEvent
    {
        [Display(Name = "Nazwa pokoju")]
        [Required(ErrorMessage = "{0} jest wymagana.")]
        [StringLength(32, ErrorMessage = "{0} musi zawierać od {2} do {1} znaków.", MinimumLength = 3)]
        public string Name { get; set; }
        
        public string Id { get; set; }
        
        public UserModel User { get; set; }
    }
}