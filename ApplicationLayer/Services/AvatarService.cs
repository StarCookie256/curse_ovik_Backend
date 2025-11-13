using PerfumeryBackend.ApplicationLayer.Interfaces;

namespace PerfumeryBackend.ApplicationLayer.Services;

public class AvatarService : IAvatarService
{
    private readonly string _avatarsFolder;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private readonly long _maxFileSize = 5 * 1024 * 1024;

    public AvatarService(IWebHostEnvironment environment)
    {
        _avatarsFolder = Path.Combine(environment.WebRootPath, "avatars");

        if (!Directory.Exists(_avatarsFolder))
        {
            Directory.CreateDirectory(_avatarsFolder);
        }
    }

    public async Task<string> SaveAvatarAsync(IFormFile avatarFile)
    {
        if (avatarFile.Length > _maxFileSize)
        {
            throw new ArgumentException($"Размер файла не должен превышать {_maxFileSize / 1024 / 1024} MB");
        }

        var extension = Path.GetExtension(avatarFile.FileName).ToLower();
        if (!_allowedExtensions.Contains(extension))
        {
            throw new ArgumentException($"Допустимые форматы: {string.Join(", ", _allowedExtensions)}");
        }

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_avatarsFolder, fileName);

        // Сохраняем файл
        using var stream = new FileStream(filePath, FileMode.Create);
        await avatarFile.CopyToAsync(stream);

        // Возвращаем относительный путь для БД
        return $"avatars/{fileName}";
    }
}
