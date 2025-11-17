using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer.Repositories.Interfaces;

namespace PerfumeryBackend.DatabaseLayer.Repositories;

public class CategoryRepository(PerfumeryDbContext context) : ICategoryRepository
{
    public async Task<List<Category>> GetCategoriesAsync() =>
        await context.Categories
            .AsNoTracking()
            .ToListAsync();

    public async Task<Category> GetCategoryByIdAsync(int id) =>
        await context.Categories
            .AsNoTracking()
            .FirstAsync(c => c.Id == id);
}
