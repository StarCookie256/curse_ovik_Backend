namespace PerfumeryBackend.ApplicationLayer.Interfaces;

public interface IAvatarService
{
    Task<string> SaveAvatarAsync(IFormFile avatarFile);
    string GetAvatarUrl(string fileName);
}
