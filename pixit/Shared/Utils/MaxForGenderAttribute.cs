using System;
using System.ComponentModel.DataAnnotations;

namespace pixit.Shared.Utils
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class MaxForGender : ValidationAttribute
    {
        public int _maxForMale;
        public int _maxForFemale;
        
        public MaxForGender(int maxForMale, int maxForFemale)
        {
            _maxForMale = maxForMale;
            _maxForFemale = maxForFemale;
        }

        protected override ValidationResult IsValid(object value, ValidationContext obj)
        {
            int gender = Convert.ToInt32(obj.ObjectType.GetProperty("Gender")?.GetValue(obj.ObjectInstance));
            int val = Convert.ToInt32(value);

            if (val < 0) obj.ObjectType.GetProperty(obj.MemberName!)?.SetValue(obj.ObjectInstance, (gender == 0 ? _maxForMale - 1 : _maxForFemale - 1));
            
            if ((gender == 0 && val >= _maxForMale) || (gender == 1) && val >= _maxForFemale)
                obj.ObjectType.GetProperty(obj.MemberName!)?.SetValue(obj.ObjectInstance, 0);
                
            return ValidationResult.Success;
        }
    }
}