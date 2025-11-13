namespace PerfumeryBackend.ApplicationLayer.Interfaces;

public interface IAvatarService
{
    Task<string> SaveAvatarAsync(IFormFile avatarFile);
}
