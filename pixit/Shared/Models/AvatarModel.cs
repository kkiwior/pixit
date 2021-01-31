using System;
using System.ComponentModel.DataAnnotations;
using pixit.Shared.Utils;

namespace pixit.Shared.Models
{
    public class AvatarModel
    {
        [Range(0, 1)]
        public int Gender { get; set; }
        
        [MaxForGender(5, 5)]
        public int Background { get; set; }
        
        [MaxForGender(4, 4)]
        public int Face { get; set; }
        
        [MaxForGender(50, 50)]
        public int Cloth { get; set; }
        
        [MaxForGender(34, 34)]
        public int Hair { get; set; }
        
        [MaxForGender(26, 17)]
        public int Mouth { get; set; }
        
        [MaxForGender(33, 50)]
        public int Eye { get; set; }
    }
}