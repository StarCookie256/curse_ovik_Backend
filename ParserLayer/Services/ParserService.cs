using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer;
using System.Collections.ObjectModel;

namespace PerfumeryBackend.ParserLayer.Services;

public class ParserService
{
    public ChromeOptions options = new ChromeOptions();
    public const string GECKO = @"D:\Games\My_projects_c#\PerfumeryBackend\bin\Debug\net8.0\";
    private ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(GECKO);
    public ChromeDriverService DriverService { get => driverService; set => driverService = value; }

    private ReadOnlyCollection<IWebElement> categoriesDiv = new([]);
    private List<IWebElement> _brandsDivList = new();
    private ReadOnlyCollection<IWebElement> _realBrandsDiv => _brandsDivList.AsReadOnly();

    public static List<string> Brands { get; } = [];
    public static List<string> Categories { get; } = [];
    public static List<string> Countries { get; } = [];
    private static Random _random = new Random();

    public Task ParseData()
    {
        options.AddArgument("--headless");

        DriverService.HideCommandPromptWindow = true;  // Скрыть консоль geckodriver

        DriverService.Start();
        Thread.Sleep(1000); // Подождать 1 секунду

        using ChromeDriver driver = new(DriverService, options);

        // парсим ЖЕНСКИЕ БРЕНДЫ
        ParseWomenBrands(driver).ForEach(item => Brands.Add(item.Text));
        int womenBrandsCount = Brands.Count;

        // парсим МУЖСКИЕ БРЕНДЫ
        ParseManBrands(driver).ForEach(item => Brands.Add(item.Text));
        int menBrandsCount = Brands.Count - womenBrandsCount;

        // парсим КАТЕГОРИИ
        ParseCategories(driver).ForEach(item => Categories.Add(item.Text));

        // парсим ТОВАРЫ из БРЕНДОВ
        int parsedBrands = 0;
        foreach (var item in _realBrandsDiv.ToList())
        {
            string brandsGender = "women";
            if (parsedBrands > womenBrandsCount)
            {
                brandsGender = "men";
            }
            string brand = item.Text;
            string productImage = string.Empty;
            string productName = string.Empty;

            if (driver.Url != $"https://духи.рф/catalog/{brandsGender}")
            {
                driver.Navigate().GoToUrl($"https://духи.рф/catalog/{brandsGender}");
            }

            //char[] brandChars = brand.ToCharArray(); // Convert to char array which is mutable

            //for (int i = 0; i < brandChars.Length; i++)
            //{
            //    if (brandChars[i] == ' ') brandChars[i] = '-';
            //}

            //brand = new string(brandChars); // Convert back to string
            item.Click();
            Thread.Sleep(100);

            var allBrandProducts = driver.FindElements(By.XPath("//div[@class='col-6 col-lg-4 mb-4']"));
            foreach (var product in allBrandProducts)
            {
                try
                {
                    IWebElement gotoPerfumePage = driver.FindElement(By.XPath($"//{product}//a[@class='p_item']"));
                    productName = gotoPerfumePage.Text;
                    gotoPerfumePage.Click();

                    Thread.Sleep(100);

                    // находим ссылку на фото продукта
                    productImage = "https://духи.рф/" + driver.FindElement(By.XPath("//div[@class='slider-singl slider-photocard mt-2 slick-initialized slick-slider']//img/@src")).GetAttribute("textContent");

                    // находим страну и сохраняем в общий массив
                    var productFlag = driver.FindElement(By.XPath("//img[@class='sflag mr-1']/..")).Text;
                    if (productFlag != null)
                    {
                        Countries.Add(productFlag);
                    }

                    // находим checkBox, отвечающий за скрытые товары
                    var productsCheckBox = driver.FindElement(By.XPath("//input[@class='checkbox_inav checkbox w-bg']"));
                    productsCheckBox?.Click();
                    // click по checkBox, смотрим товары, потом форИч по колву строчек товаров, каждый присваиваем, а потом кидаем в БД
                    var allProductsVariants = productsCheckBox == null ? driver.FindElements(By.XPath("//tr[@class=' tr_no_avl.no-stock']")) : driver.FindElements(By.XPath("//tr[@class='tr_avl']"));

                    // список с вариантами одного продукта
                    List<ProductVariation> productVariants = new List<ProductVariation>();

                    if (allProductsVariants != null)
                    {
                        foreach (IWebElement webProductVariant in allProductsVariants)
                        {
                            var productVarName = webProductVariant.FindElement(By.XPath(".//div[@class='d-flex flex-column table-filter-description']/span"));
                            var productVarPrice = webProductVariant.FindElement(By.XPath(".//span[@itemprop='price']"));
                            var productVarVolume = webProductVariant.FindElement(By.XPath(".//div[@class='table_volume pl-2']/span"));

                            ProductVariation productVariant = new ProductVariation()
                            {
                                Id = 0,
                                ProductId = 0,
                                Price = Convert.ToDouble(productVarPrice.Text),
                                Volume = Convert.ToDouble(productVarVolume.Text),
                                Stock = _random.Next(42, 10000)
                            };

                            productVariants.Add(productVariant);
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

    public List<IWebElement> ParseCategories(ChromeDriver driver)
    {
        driver.Navigate().GoToUrl("https://духи.рф/catalog");

        categoriesDiv = driver.FindElements(By.XPath("//div[contains(@class,'card-header')][contains(., 'Парфюмерия')]/following-sibling::div[contains(@class,'aside_type_wrapper')]//div[@class='mCSB_container']//input[@type='checkbox']"));
        return categoriesDiv.ToList();
    }

}
