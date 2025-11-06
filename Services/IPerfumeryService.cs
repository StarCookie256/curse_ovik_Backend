using PerfumeryBackend.PerfumeryDatabase;

namespace PerfumeryBackend.Services
{
    public interface IPerfumeryService
    {
        public Task BrandsToDB(List<string> brands);
        public Task CategoriesToDB(List<string> categories);
        public Task DataToDB(PerfumeData perfumeData);
        public Task<JsonContent> TakeDataFromDB();
    }
}
