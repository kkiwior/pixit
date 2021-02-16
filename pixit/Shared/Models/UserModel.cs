using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Newtonsoft.Json;
using pixit.Shared.Utils;

namespace pixit.Shared.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        [Display(Name = "Nazwa użytkownika")]
        [Required(ErrorMessage = "{0} jest wymagana.")]
        [StringLength(32, ErrorMessage = "{0} musi zawierać od {2} do {1} znaków.", MinimumLength = 3)]
        public string Name { get; set; }
        
        [JsonIgnore]
        public string Token { get; set; }

        [JsonIgnore] 
        public List<CardModel> CardDeck { get; set; } = new();

        [NestedObjectValidation]
        public AvatarModel Avatar { get; set; } = new();

        public void Validate()
        {
            ValidationContext vc = new ValidationContext(Avatar);
            Validator.TryValidateObject(Avatar, vc, null, true);
        }
    }
}