
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V135.HeadlessExperimental;
using OpenQA.Selenium.Chrome;
using PerfumeryBackend;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer;
using PerfumeryBackend.ParserLayer.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace PerfumeryBackend.Services
{
    public class InitializationService : IInitializationService
    {
        private readonly PerfumeryDbContext _db = new PerfumeryDbContext();

        public Task DataToDB()
        {
            BrandsToDB(ParserService.Brands);
            CategoriesToDB(ParserService.Categories);
            CountriesToDB(ParserService.Countries);

            return Task.CompletedTask;
        }

        public Task BrandsToDB(List<string> importBrands)
        {
            var brands = importBrands;

            foreach (string brand in brands)
            {
                // если нет в таблице - добавляем
                if (!_db.Brands.Any(e => e.Name == brand))
                {
                    _db.Brands.Add(new Brand { Name = brand });
                    _db.SaveChanges();
                }

            }

            return Task.CompletedTask;
        }

        public Task CategoriesToDB(List<string> importCategories)
        {
            var categories = importCategories;

            foreach (string category in categories)
            {
                // если нет в таблице - добавляем
                if (!_db.Categories.Any(e => e.Name == category))
                {
                    _db.Categories.Add(new Category { Name = category });
                    _db.SaveChanges();
                }

            }

            return Task.CompletedTask;
        }

        public Task CountriesToDB(List<string> importCountries)
        {
            var countries = importCountries;

            foreach (string country in countries)
            {
                // если нет в таблице - добавляем
                if (!_db.Countries.Any(e => e.Name == country))
                {
                    _db.Countries.Add(new Country { Name = country });
                    _db.SaveChanges();
                }

            }

            return Task.CompletedTask;
        }
    }
}

// добавление записей в БД
//public void AddDataToDb()
//{
//    using PerfumeryDbContext db = new PerfumeryDbContext();

//    AddBrandsToDb(db);
//}

//// бренды в БД
//public void AddBrandsToDb(PerfumeryDbContext db)
//{
//    var brands = WebsiteParser.brands;

//    foreach (string brand in brands)
//    {
//        // если нет в таблице - добавляем
//        if (!db.Brands.Any(e => e.Name == brand))
//        {
//            db.Brands.Add(new Brand { Name = brand });
//        }

//    }
//}

//// категории в БД
//public void AddCategoriesToDb(PerfumeryDbContext db)
//{
//    var categories = WebsiteParser.categories;

//    foreach (string item in categories)
//    {
//        // если нет в таблице - добавляем
//        if (!db.Categories.Any(e => e.Name == item))
//        {
//            db.Categories.Add(new Category { Name = item });
//        }

//    }
//}
