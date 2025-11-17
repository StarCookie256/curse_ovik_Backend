using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(int id);
}