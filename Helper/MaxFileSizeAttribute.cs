using System.ComponentModel.DataAnnotations;

public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly int _maxFileSize;
    public MaxFileSizeAttribute(int mb) => _maxFileSize = mb * 1024 * 1024;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        if (value is IEnumerable<IFormFile> files)
        {
            foreach (var file in files)
            {
                if (file.Length > _maxFileSize)
                    return new ValidationResult($"File {file.FileName} melebihi ukuran maksimal {_maxFileSize / 1024 / 1024}MB.");
            }
        }
        else if (value is IFormFile file)
        {
            if (file.Length > _maxFileSize)
                return new ValidationResult($"File melebihi ukuran maksimal {_maxFileSize / 1024 / 1024}MB.");
        }
        return ValidationResult.Success;
    }
}