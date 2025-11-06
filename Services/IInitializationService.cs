using PerfumeryBackend;

namespace PerfumeryBackend.Services
{
    public interface IInitializationService
    {
        public Task BrandsToDB(List<string> brands);
        public Task CategoriesToDB(List<string> categories);
        public Task CountriesToDB(List<string> countries);
        public Task DataToDB();
        //public Task<JsonContent> TakeDataFromDB();
    }
}
