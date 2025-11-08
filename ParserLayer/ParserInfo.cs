using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.ParserLayer
{
    public class ParserInfo
    {
        public static List<string> Brands { get; } = [];
        public static List<Country> Countries { get; } = [];
        public static List<ProductVariation> ProductVariants { get; } = new List<ProductVariation>();
        public static List<Product> Products { get; } = new List<Product>();
        public static List <Category> Categories { get; } = new List<Category>();
    }
}
