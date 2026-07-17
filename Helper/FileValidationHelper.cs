public static class FileValidationHelper
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public static (bool IsValid, string ErrorMessage) ValidateImage(IFormFile file, string fieldName)
    {
        if (file == null) return (true, ""); // Anggap valid jika opsional

        // Validasi Ukuran
        if (file.Length > MaxFileSize)
        {
            return (false, $"Ukuran {fieldName} tidak boleh lebih dari 5MB.");
        }

        // Validasi Ekstensi
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!AllowedExtensions.Contains(extension))
        {
            return (false, $"Format {fieldName} tidak valid. Hanya diperbolehkan .jpg, .jpeg, .png, .gif.");
        }

        return (true, "");
    }
}