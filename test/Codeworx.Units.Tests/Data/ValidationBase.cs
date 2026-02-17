using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Units.Tests.Data
{
    internal class ValidationBase
    {
        public bool Validate(out List<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(this, new ValidationContext(this), results, true);
        }
    }
}