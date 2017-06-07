using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebGameOfLife.Attributes
{
    public class CellsLengthAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var temp = (UserTemplate)validationContext.ObjectInstance;

            if (temp.Cells.Length != temp.Height * temp.Width)
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            else
                return null;

        }
    }
}