using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.ApplicationLayer.Interfaces;

public interface ICategoryService
{
    Task<List<Category>> GetCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(int id);
}
