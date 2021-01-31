using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace pixit.Shared.Models
{
    public class RoomModel
    {
        public string Name { get; set; }
        [Display(Name = "Liczba graczy")]
        [Range(2, 20, ErrorMessage = "{0} musi zawierać się pomiędzy {1} i {2}." )]
        public int Slots { get; set; } = 5;
        
        [Display(Name = "Liczba punktów")]
        [Range(5, 100, ErrorMessage = "{0} musi zawierać się pomiędzy {1} i {2}." )]
        public int MaxScore { get; set; } = 10;
        
        public List<UserModel> Users { get; set; } = new();

        public int UsersOnline
        {
            get => Users.Count;
        }
        public int CardsCount { get; set; }
        public string Host { get; set; }
        public bool Started { get; set; }
    }
}
