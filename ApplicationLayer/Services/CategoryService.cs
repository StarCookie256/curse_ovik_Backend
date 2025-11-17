using PerfumeryBackend.ApplicationLayer.Interfaces;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.ApplicationLayer.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<List<Category>> GetCategoriesAsync() =>
        await categoryRepository.GetCategoriesAsync();

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(id);

        if (category == null)
        {
            throw new Exception("Category not found!");
        }

        return category;
    }
}
