using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Resxul.Properties;

namespace Resxul
{
    [AttributeUsage(AttributeTargets.Property)]
    class ToolsExistValidation : ValidationAttribute
    {
        public static bool CheckToolsExistence(string folderPath)
        {
            try
            {
                return File.Exists(Path.Combine(folderPath, Global.Resgen)) &
                       File.Exists(Path.Combine(folderPath, Global.Al));
            }
            catch
            {
                return false;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[] memberNames = { validationContext.MemberName };

            var s = value as string;

            if (string.IsNullOrEmpty(s))
                return new ValidationResult(ErrorMessage, memberNames);

            return CheckToolsExistence(s)
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage, memberNames);
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    class FileExistsValidation : ValidationAttribute
    {
        public bool AllowNullOrEmpty { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[] memberNames = { validationContext.MemberName };

            var s = value as string;

            if (string.IsNullOrEmpty(s))
                return AllowNullOrEmpty ? ValidationResult.Success : new ValidationResult(Resources.Validation_FieldRequired, memberNames);

            try
            {
                return File.Exists(s) ? ValidationResult.Success : new ValidationResult(Resources.Validation_FileNotFound, memberNames);
            }
            catch (Exception)
            {
                return new ValidationResult(Resources.Validation_FileNotFound, memberNames);
            }
        }
    }
}
