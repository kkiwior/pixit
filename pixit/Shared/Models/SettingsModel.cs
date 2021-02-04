using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace pixit.Shared.Models
{
    public class SettingsModel
    {
        [Display(Name = "Liczba graczy")]
        [Range(2, 20, ErrorMessage = "{0} musi zawierać się pomiędzy {1} i {2}." )]
        public int Slots { get; set; }
        
        [Display(Name = "Liczba punktów")]
        [Range(5, 100, ErrorMessage = "{0} musi zawierać się pomiędzy {1} i {2}." )]
        public int MaxScore { get; set; }
        public int CardsCount { get; set; }
        
        [JsonIgnore]
        public string Host { get; set; }

        public SettingsModel()
        {
            MaxScore = 10;
            Slots = 5;
        }
    }
}