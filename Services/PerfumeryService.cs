
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V135.HeadlessExperimental;
using OpenQA.Selenium.Chrome;
using PerfumeryBackend.PerfumeryDatabase;

namespace PerfumeryBackend.Services
{
    public class PerfumeryService : IPerfumeryService
    {

        public Task CategoriesToDB(List<string> categories)
        {
            throw new NotImplementedException();
        }

        public Task DataToDB(PerfumeData perfumeData)
        {
            //что-то тут будет в будущем

            return Task.CompletedTask;
        }

        public Task<JsonContent> TakeDataFromDB()
        {
            throw new NotImplementedException();
        }

        public Task BrandsToDB(List<string> brands)
        {
            throw new NotImplementedException();
        }
    }
}
