using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;

namespace PerfumeryBackend.ParserLayer.Services;

public class ParserService
{
    // БД
    private readonly PerfumeryDbContext _db = new PerfumeryDbContext();

    // НАСТРОЙКИ ПАРСЕРА
    public ChromeOptions options = new ChromeOptions();
    public const string GECKO = @"D:\Games\My_projects_c#\PerfumeryBackend\bin\Debug\net8.0\";
    private ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(GECKO);
    public ChromeDriverService DriverService { get => driverService; set => driverService = value; }

    private ReadOnlyCollection<IWebElement> categoriesDiv = new([]);
    private List<IWebElement> _brandsDivList = new();
    private ReadOnlyCollection<IWebElement> _realBrandsDiv => _brandsDivList.AsReadOnly();
    private static Random _random = new Random();

    public Task ParseData()
    {
        options.AddArgument("--headless");

        DriverService.HideCommandPromptWindow = true;  // Скрыть консоль geckodriver

        DriverService.Start();
        Thread.Sleep(1000); // Подождать 1 секунду

        using ChromeDriver driver = new(DriverService, options);

        // парсим ЖЕНСКИЕ БРЕНДЫ
        ParseWomenBrands(driver).ForEach(item => ParserInfo.Brands.Add(item.Text));
        int womenBrandsCount = ParserInfo.Brands.Count;

        // парсим МУЖСКИЕ БРЕНДЫ
        ParseManBrands(driver).ForEach(item => ParserInfo.Brands.Add(item.Text));
        int menBrandsCount = ParserInfo.Brands.Count - womenBrandsCount;

        // парсим КАТЕГОРИИ
        ParseCategories(driver);

        // парсим ТОВАРЫ из БРЕНДОВ
        int parsedBrands = 0;
        foreach (var item in _realBrandsDiv.ToList())
        {
            // делаем БРЕНД и СОХРАНЯЕМ В БД
            string brandName = item.Text;
            Brand brand = new Brand()
            {
                Name = brandName
            };
            _db.Brands.Add(brand);
            _db.SaveChanges();

            string brandsGender = "women";
            if (parsedBrands > womenBrandsCount)
            {
                brandsGender = "men";
            }

            if (driver.Url != $"https://духи.рф/catalog/{brandsGender}")
            {
                driver.Navigate().GoToUrl($"https://духи.рф/catalog/{brandsGender}");
            }
            
            // ПЕРЕХОД на страницу БРЕНДа
            item.Click();
            Thread.Sleep(100);

            // ПАРСИМ САМ ТОВАР У БРЕНДА
            var allBrandProducts = driver.FindElements(By.XPath("//div[@class='col-6 col-lg-4 mb-4']"));
            foreach (var product in allBrandProducts)
            {
                try
                {
                    string productImage = string.Empty;
                    string productName = string.Empty;
                    string productExpDate = $"{_random.Next(16, 72)} месяцев с момента апробирования";
                    string productGender = brandsGender == "women" ? "female" : "male";
                    int productManufactureYear = 2006;

                    IWebElement gotoPerfumePage = driver.FindElement(By.XPath($"//{product}//a[@class='p_item']"));
                    productName = gotoPerfumePage.Text;
                    gotoPerfumePage.Click();

                    Thread.Sleep(100);

                    // находим ссылку на фото продукта
                    productImage = "https://духи.рф/" + driver.FindElement(By.XPath("//div[@class='slider-singl slider-photocard mt-2 slick-initialized slick-slider']//img/@src")).GetAttribute("textContent");

                    // НАХОДИМ СРОК ГОДНОСТИ или нет
                    string manufacYearStr = string.Empty;
                    manufacYearStr = driver.FindElement(By.XPath("//div[@class='slider-singl slider-photocard mt-2 slick-initialized slick-slider']//img/@src")).Text ?? manufacYearStr;
                    productManufactureYear = int.Parse(manufacYearStr);

                    // ИЩЕМ СТРАНУ, СОХРАНЯЕМ
                    var productFlag = driver.FindElement(By.XPath("//img[@class='sflag mr-1']/..")).Text;
                    Country countryObject = new();
                    if (productFlag != null)
                    {
                        countryObject.Name = productFlag;
                        if (!_db.Countries.Any(e => e.Name == countryObject.Name))
                        {
                            _db.Countries.Add(countryObject);
                            _db.SaveChanges();
                            ParserInfo.Countries.Add(countryObject);
                        }
                        else
                        {
                            countryObject = ParserInfo.Countries.Find(x => x.Name == countryObject.Name) ?? countryObject;
                        }
                        
                    }

                    // Сохраняем ТОВАР в БД и МАССИВ
                    Product newProduct = new Product()
                    {
                        Image = productImage,
                        Name = productName,
                        CountryId = countryObject.Id,
                        BrandId = brand.Id,
                        ExpirationDate = productExpDate,
                        Gender = productGender,
                        ManufactureYear = productManufactureYear
                    };
                    if (!_db.Products.Any(e => e.Name == newProduct.Name && e.Image == newProduct.Image))
                    {
                        _db.Products.Add(newProduct);
                        _db.SaveChanges();
                        ParserInfo.Products.Add(newProduct);
                    }
                    
                    // РАБОТА с ВАРИАНТАМИ

                    // находим checkBox, отвечающий за скрытые товары
                    var productsCheckBox = driver.FindElement(By.XPath("//input[@class='checkbox_inav checkbox w-bg']"));
                    productsCheckBox?.Click();
                    // click по checkBox, смотрим товары, потом форИч по колву строчек товаров, каждый присваиваем, а потом кидаем в БД
                    var allProductsVariants = productsCheckBox == null ? driver.FindElements(By.XPath("//tr[@class=' tr_no_avl.no-stock']")) : driver.FindElements(By.XPath("//tr[@class='tr_avl']"));

                    // список с вариантами одного продукта
                    if (allProductsVariants != null)
                    {
                        foreach (IWebElement webProductVariant in allProductsVariants)
                        {
                            Category productVarCategory = new() { Id = _random.Next(1, ParserInfo.Categories.Count-1) };
                            var findedVarCategory = webProductVariant.FindElement(By.XPath(".//div[@class='d-flex flex-column table-filter-description']/span"));
                            if (findedVarCategory != null) 
                            {
                                productVarCategory = ParserInfo.Categories.Find(x => x.Name == findedVarCategory.Text) ?? productVarCategory;
                            }
                            var findedVarPrice = webProductVariant.FindElement(By.XPath(".//span[@itemprop='price']"));
                            var findedVarVolume = webProductVariant.FindElement(By.XPath(".//div[@class='table_volume pl-2']/span"));

                            double productVarPrice = Convert.ToDouble(findedVarPrice.Text.Split('р')[0]);
                            double productVarVolume = Convert.ToDouble(findedVarVolume.Text.Split('м')[0]);

                            ProductVariation productVariant = new ProductVariation()
                            {
                                ProductId = newProduct.Id,
                                CategoryId = productVarCategory.Id,
                                Price = productVarPrice,
                                Volume = productVarVolume,
                                Stock = _random.Next(42, 10000)
                            };
                            if (!_db.ProductVariations.Any(e => e.CategoryId == productVariant.CategoryId && e.ProductId == productVariant.ProductId 
                            && e.Volume == productVariant.Volume))
                            {
                                _db.ProductVariations.Add(productVariant);
                                _db.SaveChanges();
                                ParserInfo.ProductVariants.Add(productVariant);
                            }
                        }

                    }
                }
                catch (ArgumentNullException ex)
                {
                    throw new ArgumentException(ex.Message);
                }
                finally
                {
                    parsedBrands++;
                }
            }
        }

        return Task.CompletedTask;
    }


    public List<IWebElement> ParseWomenBrands(ChromeDriver driver)
    {
        driver.Navigate().GoToUrl("https://духи.рф/catalog");

        var womenBrands = driver.FindElements(By.XPath("//div[@class='union_brands_list d-flex flex-row flex-wrap w-100']//div[@class='col-11']//a[@class='wrap_a items type_name']"));

        _brandsDivList.AddRange(womenBrands);
        return womenBrands.ToList();
    }

    public List<IWebElement> ParseManBrands(ChromeDriver driver)
    {
        driver.Navigate().GoToUrl("https://духи.рф/catalog/men");

        var manBrands = driver.FindElements(By.XPath("//div[@class='union_brands_list d-flex flex-row flex-wrap w-100']//div[@class='col-11']//a[@class='wrap_a items type_name']"));

        _brandsDivList.AddRange(manBrands);
        return manBrands.ToList();
    }

    public void ParseCategories(ChromeDriver driver)
    {
        driver.Navigate().GoToUrl("https://духи.рф/catalog");

        categoriesDiv = driver.FindElements(By.XPath("//div[contains(@class,'card-header')][contains(., 'Парфюмерия')]/following-sibling::div[contains(@class,'aside_type_wrapper')]//div[@class='mCSB_container']//input[@type='checkbox']"));
        List<IWebElement> categoriesList = categoriesDiv.ToList();

        foreach (var category in categoriesList) 
        {
            var localCategory = new Category() { Name = category.Text };
            if (!_db.Categories.Any(e => e.Name == localCategory.Name))
            {
                _db.Categories.Add(localCategory);
                _db.SaveChanges();
                ParserInfo.Categories.Add(localCategory);
            }
        }
    }

}



//using OpenQA.Selenium;
//using OpenQA.Selenium.DevTools.V135.HeadlessExperimental;
//using OpenQA.Selenium.Chrome;
//using PerfumeryBackend;
//using PerfumeryBackend.DatabaseLayer.Models;
//using PerfumeryBackend.DatabaseLayer;
//using PerfumeryBackend.ParserLayer.Services;
//using Microsoft.AspNetCore.Http.HttpResults;
//using PerfumeryBackend.ParserLayer;

//namespace PerfumeryBackend.Services
//{
//    public class InitializationService : IInitializationService
//    {
//        private readonly PerfumeryDbContext _db = new PerfumeryDbContext();

//        public Task DataToDB()
//        {
//            BrandsToDB(ParserInfo.Brands);
//            CategoriesToDB(ParserInfo.Categories);
//            CountriesToDB(ParserInfo.Countries);

//            return Task.CompletedTask;
//        }

//        public Task BrandsToDB(List<string> importBrands)
//        {
//            var brands = importBrands;

//            foreach (string brand in brands)
//            {
//                // если нет в таблице - добавляем
//                if (!_db.Brands.Any(e => e.Name == brand))
//                {
//                    _db.Brands.Add(new Brand { Name = brand });
//                    _db.SaveChanges();
//                }

//            }

//            return Task.CompletedTask;
//        }

//        public Task CountriesToDB(List<string> importCountries)
//        {
//            var countries = importCountries;

//            foreach (string country in countries)
//            {
//                // если нет в таблице - добавляем
//                if (!_db.Countries.Any(e => e.Name == country))
//                {
//                    _db.Countries.Add(new Country { Name = country });
//                    _db.SaveChanges();
//                }

//            }

//            return Task.CompletedTask;
//        }
//    }
//}

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
