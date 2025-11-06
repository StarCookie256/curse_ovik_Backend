using PerfumeryBackend.DatabaseLayer;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.ParserLayer.Services;
using System.Linq;

namespace PerfumeryBackend.ParserLayer;

public class DbManipulations
{
    // бубубу
    // добавление записей в БД
    public void AddDataToDb()
    {
        using PerfumeryDbContext db = new PerfumeryDbContext();

        AddBrandsToDb(db);
    }

    // бренды в БД
    public void AddBrandsToDb(PerfumeryDbContext db)
    {
        var brands = WebsiteParser.brands;

        foreach (string brand in brands)
        {
            // если нет в таблице - добавляем
            if (!db.Brands.Any(e => e.Name == brand))
            {
                db.Brands.Add(new Brand { Name = brand });
            }

        }
    }

    // категории в БД
    public void AddCategoriesToDb(PerfumeryDbContext db)
    {
        var categories = WebsiteParser.categories;

        foreach (string item in categories)
        {
            // если нет в таблице - добавляем
            if (!db.Categories.Any(e => e.Name == item))
            {
                db.Categories.Add(new Category { Name = item });
            }

        }
    }

}


// проверка в БД на существующее поле
//public bool DbValidation()
//{
//    return true;
//}

