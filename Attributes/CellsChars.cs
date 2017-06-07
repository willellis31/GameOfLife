using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebGameOfLife.Attributes
{
    public class CellsCharsAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var temp = (UserTemplate)validationContext.ObjectInstance;


            foreach (char letter in temp.Cells)
            {
                if(char.ToLower(letter) != 'o' && char.ToLower(letter) != 'x')
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }
            return null;

        }
    }
}