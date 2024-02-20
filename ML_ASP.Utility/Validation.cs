using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ML_ASP.Utility
{
    public class Validation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var password = (string)value;

            bool _hasUppercase = hasUpperCase(password);

            if (!_hasUppercase)
            {
                ErrorMessage = "Password must contain at least one uppercase letter.";
                return false;
            }

            return true;
        }

        private bool hasUpperCase(string password)
        {
            return Regex.IsMatch(password, @"[A-Z]");
        }
    }
}
