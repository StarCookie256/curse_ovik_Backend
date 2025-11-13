using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PerfumeryBackend.DatabaseLayer.Models;
using PerfumeryBackend.DatabaseLayer;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using PerfumeryBackend.ParserLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PerfumeryBackend.ParserLayer.Services;

public class ParserService(PerfumeryDbContext _db) : IParserService
{
    // НАСТРОЙКИ ПАРСЕРА
    public ChromeOptions options = new ChromeOptions();
    public const string GECKO = @"D:\Games\My projects c#\PerfumeryBackend\bin\Debug\net8.0\chromedriver.exe";
    private ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(GECKO);
    public ChromeDriverService DriverService { get => driverService; set => driverService = value; }

    private ReadOnlyCollection<IWebElement> categoriesDiv = new([]);
    private List<IWebElement> _brandsDivList = new();
    private ReadOnlyCollection<IWebElement> _realBrandsDiv => _brandsDivList.AsReadOnly();
    private readonly static Random _random = new Random();

    public async Task ParseData()
    {
        options.AddArgument("--headless");
        DriverService.HideCommandPromptWindow = true;

        DriverService.Start();
        await Task.Delay(1000);

        using ChromeDriver driver = new(DriverService, options);

        try
        {
            // Парсим бренды и получаем их названия сразу
            var brandNames = await ParseAllBrands(driver);

            // Парсим категории
            await ParseCategories(driver);

            // Парсим товары из брендов
            await ParseProductsFromBrands(driver, brandNames);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Критическая ошибка: {ex.Message}");
            throw;
        }
    }

    private async Task<List<string>> ParseAllBrands(ChromeDriver driver)
    {
        var allBrandNames = new List<string>();

        // Женские бренды (первые 350)
        var womenBrands = await GetBrandNamesFromPage(driver, "https://духи.рф/catalog");
        var womenBrandsToSave = womenBrands.Take(350).ToList(); // Берем только 350
        allBrandNames.AddRange(womenBrandsToSave);
        Console.WriteLine($"✅ Найдено женских брендов: {womenBrands.Count}, сохраняем: {womenBrandsToSave.Count}");

        // Мужские бренды (первые 350)  
        var menBrands = await GetBrandNamesFromPage(driver, "https://духи.рф/catalog/men");
        var menBrandsToSave = menBrands.Take(350).ToList(); // Берем только 350
        allBrandNames.AddRange(menBrandsToSave);
        Console.WriteLine($"✅ Найдено мужских брендов: {menBrands.Count}, сохраняем: {menBrandsToSave.Count}");

        // Сохраняем бренды в БД
        foreach (var brandName in allBrandNames)
        {
            if (!string.IsNullOrWhiteSpace(brandName))
            {
                var brand = new Brand() { Name = brandName };
                if (!await _db.Brands.AnyAsync(e => e.Name == brand.Name))
                {
                    await _db.Brands.AddAsync(brand);
                    await _db.SaveChangesAsync();
                    Console.WriteLine($"✅ Сохранен бренд: {brandName}");
                }
            }
        }

        return allBrandNames;
    }

    private async Task<List<string>> GetBrandNamesFromPage(ChromeDriver driver, string url)
    {
        driver.Navigate().GoToUrl(url);
        await Task.Delay(3000);

        var brandNames = new List<string>();

        try
        {
            // Используем JavaScript для получения всех названий брендов сразу
            var script = @"
                var brands = [];
                var elements = document.querySelectorAll('div.union_brands_list div.col-11 a.wrap_a.items.type_name');
                for (var i = 0; i < elements.length; i++) {
                    brands.push(elements[i].textContent?.trim() || '');
                }
                return brands;
            ";

            var scriptResult = ((IJavaScriptExecutor)driver).ExecuteScript(script);
            var result = scriptResult as IReadOnlyCollection<object> ?? Array.Empty<object>();

            foreach (var brandNameObj in result)
            {
                if (brandNameObj?.ToString() is string brandName && !string.IsNullOrWhiteSpace(brandName))
                {
                    brandNames.Add(brandName);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Ошибка при парсинге брендов: {ex.Message}");
        }

        return brandNames;
    }

    private async Task ParseProductsFromBrands(ChromeDriver driver, List<string> brandNames)
    {
        int parsedBrands = 0;
        int womenBrandsCount = brandNames.Count / 2; // Примерное разделение

        foreach (var brandName in brandNames)
        {
            if (parsedBrands > 700) break;

            try
            {
                Console.WriteLine($"🔹 Обрабатываем бренд: {brandName} ({parsedBrands + 1}/{brandNames.Count})");

                // Определяем пол бренда
                string brandsGender = parsedBrands < womenBrandsCount ? "women" : "men";
                string genderUrl = $"https://духи.рф/catalog/{brandsGender}";

                // Переходим на страницу бренда
                driver.Navigate().GoToUrl(genderUrl);
                await Task.Delay(2000);

                // Ищем и кликаем по бренду
                if (!await ClickOnBrand(driver, brandName))
                {
                    Console.WriteLine($"⚠️ Не удалось найти бренд: {brandName}");
                    parsedBrands++;
                    continue;
                }

                await Task.Delay(1000);

                // Парсим товары бренда
                await ParseBrandProducts(driver, brandName, brandsGender);

                parsedBrands++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка при обработке бренда '{brandName}': {ex.Message}");
                parsedBrands++;
            }
        }
    }

    private async Task<bool> ClickOnBrand(ChromeDriver driver, string brandName)
    {
        try
        {
            // Ищем бренд по названию
            var brandElements = driver.FindElements(By.XPath($"//a[contains(@class, 'wrap_a') and contains(text(), '{brandName}')]"));

            if (brandElements.Count > 0)
            {
                brandElements[0].Click();
                await Task.Delay(500);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Ошибка при клике на бренд '{brandName}': {ex.Message}");
        }

        return false;
    }

    private async Task ParseBrandProducts(ChromeDriver driver, string brandName, string gender)
    {
        try
        {
            // Находим все товары на странице
            var productElements = driver.FindElements(By.XPath("//div[@class='col-6 col-lg-4 mb-4']"));
            Console.WriteLine($"📦 Найдено товаров у бренда {brandName}: {productElements.Count}");

            int parsedProducts = 0;
            foreach (var productElement in productElements.Take(3)) // Берем первые 3 товара
            {
                if (parsedProducts >= 3) break;

                try
                {
                    // ВОТ ТУТ ВЫЗЫВАЕМ ParseSingleProduct
                    await ParseSingleProduct(driver, productElement, brandName, gender);
                    parsedProducts++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Ошибка при парсинге товара: {ex.Message}");
                    parsedProducts++;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при парсинге товаров бренда '{brandName}': {ex.Message}");
        }
    }

    private async Task ParseSingleProduct(ChromeDriver driver, IWebElement productElement, string brandName, string gender)
    {
        try
        {
            // Получаем название СРАЗУ и работаем только с ним
            string productName = productElement.FindElement(By.XPath(".//a[@class='p_item']")).Text;
            Console.WriteLine($"🛍️ Обрабатываем товар: {productName}");

            // Кликаем через JavaScript (менее подвержено StaleElement)
            var link = productElement.FindElement(By.XPath(".//a[@class='p_item']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", link);

            await Task.Delay(2000);

            // Парсим детали
            await ParseProductDetails(driver, productName, brandName, gender);

            // Возвращаемся назад
            ((IJavaScriptExecutor)driver).ExecuteScript("window.history.back()");
            await Task.Delay(1000);
        }
        catch (StaleElementReferenceException)
        {
            Console.WriteLine("⚠️ Stale element, пропускаем товар...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при обработке товара: {ex.Message}");
        }
    }

    private async Task ParseProductDetails(ChromeDriver driver, string productName, string brandName, string gender)
    {
        try
        {
            // Парсим изображение
            string productImage = "";

            // Пробуем основные селекторы
            var imageElement = await Task.Run(() =>
                driver.FindElements(By.XPath("//div[contains(@class,'slider-singl')]//img")).FirstOrDefault() ??
                driver.FindElements(By.XPath("//a[contains(@class,'store_single_img')]//img")).FirstOrDefault()
            );

            if (imageElement != null)
            {
                productImage = imageElement.GetAttribute("src") ?? "";
            }

            if (!string.IsNullOrEmpty(productImage) && productImage.StartsWith("/"))
            {
                productImage = "https://духи.рф" + productImage;
            }

            // Парсим страну
            string countryName = "";
            try
            {
                var countryElement = await Task.Run(() => driver.FindElement(By.XPath("//img[@class='sflag mr-1']/..")));
                countryName = await Task.Run(() => countryElement.Text);
            }
            catch { /* Страна не найдена */ }

            // Сохраняем в БД асинхронно
            await SaveProductToDatabase(productName, productImage, countryName, brandName, gender);

            Console.WriteLine($"✅ Обработан товар: {productName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при парсинге деталей товара '{productName}': {ex.Message}");
        }
    }

    private async Task SaveProductToDatabase(string productName, string productImage, string countryName, string brandName, string gender)
    {
        // Сохраняем страну
        Country countryObject = new Country { Name = countryName };
        if (!string.IsNullOrWhiteSpace(countryName))
        {
            var existingCountry = await _db.Countries.FirstOrDefaultAsync(c => c.Name == countryName);
            if (existingCountry == null)
            {
                await _db.Countries.AddAsync(countryObject);
                await _db.SaveChangesAsync();
            }
            else
            {
                countryObject = existingCountry;
            }
        }

        // Сохраняем товар
        var brand = await _db.Brands.FirstOrDefaultAsync(b => b.Name == brandName);
        if (brand != null)
        {
            var product = new Product
            {
                Image = productImage,
                Name = productName,
                CountryId = countryObject.Id,
                BrandId = brand.Id,
                ExpirationDate = $"{_random.Next(16, 72)} месяцев с момента апробирования",
                Gender = gender == "women" ? "female" : "male",
                ManufactureYear = _random.Next(2006, 2024)
            };

            if (!await _db.Products.AnyAsync(p => p.Name == product.Name && p.Image == product.Image))
            {
                await _db.Products.AddAsync(product);
                await _db.SaveChangesAsync();
            }
        }
    }

    public async Task ParseCategories(ChromeDriver driver)
    {
        driver.Navigate().GoToUrl("https://духи.рф/catalog");
        await Task.Delay(3000);

        try
        {
            // ПРОСТОЙ И НАДЕЖНЫЙ СЕЛЕКТОР
            var script = @"
            var categories = [];
            // Ищем все контейнеры с категориями
            var headers = document.querySelectorAll('div.card-header');
            var perfumeHeader = null;
            
            // Находим заголовок 'Парфюмерия'
            for (var i = 0; i < headers.length; i++) {
                if (headers[i].textContent.includes('Парфюмерия')) {
                    perfumeHeader = headers[i];
                    break;
                }
            }
            
            if (perfumeHeader && perfumeHeader.nextElementSibling) {
                var container = perfumeHeader.nextElementSibling;
                var inputs = container.querySelectorAll('input[type=""checkbox""]');
                
                for (var j = 0; j < inputs.length; j++) {
                    var input = inputs[j];
                    var label = input.closest('label');
                    if (label) {
                        categories.push(label.textContent.trim());
                    } else {
                        // Ищем текст рядом с checkbox
                        var span = input.nextElementSibling;
                        if (span && span.classList.contains('checkbox-text')) {
                            categories.push(span.textContent.trim());
                        }
                    }
                }
            }
            return categories;
        ";

            var scriptResult = ((IJavaScriptExecutor)driver).ExecuteScript(script);
            var result = scriptResult as IReadOnlyCollection<object> ?? Array.Empty<object>();

            Console.WriteLine($"📂 Найдено категорий: {result.Count}");

            foreach (var categoryNameObj in result)
            {
                if (categoryNameObj?.ToString() is string categoryName && !string.IsNullOrWhiteSpace(categoryName))
                {
                    var localCategory = new Category() { Name = categoryName };

                    if (!await _db.Categories.AnyAsync(e => e.Name == localCategory.Name))
                    {
                        await _db.Categories.AddAsync(localCategory);
                        await _db.SaveChangesAsync();
                        Console.WriteLine($"✅ Добавлена категория: {categoryName}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при парсинге категорий: {ex.Message}");

            // Альтернативный способ - через Selenium
            await ParseCategoriesAlternative(driver);
        }
    }

    // Альтернативный метод если JS не работает
    private async Task ParseCategoriesAlternative(ChromeDriver driver)
    {
        try
        {
            Console.WriteLine("🔄 Пробуем альтернативный метод парсинга категорий...");

            // Ищем заголовок "Парфюмерия"
            var headers = driver.FindElements(By.XPath("//div[contains(@class, 'card-header')]"));
            IWebElement perfumeHeader = null;

            foreach (var header in headers)
            {
                if (header.Text.Contains("Парфюмерия"))
                {
                    perfumeHeader = header;
                    break;
                }
            }

            if (perfumeHeader != null)
            {
                // Находим следующий элемент (контейнер с категориями)
                var container = perfumeHeader.FindElement(By.XPath("./following-sibling::div[contains(@class, 'aside_type_wrapper')]"));
                var categoryInputs = container.FindElements(By.XPath(".//input[@type='checkbox']"));

                Console.WriteLine($"📂 Найдено категорий (альтернативный метод): {categoryInputs.Count}");

                foreach (var input in categoryInputs)
                {
                    try
                    {
                        // Получаем текст из ближайшего label
                        var label = input.FindElement(By.XPath("./ancestor::label[1]"));
                        var categoryName = label.Text.Trim();

                        if (!string.IsNullOrEmpty(categoryName))
                        {
                            var localCategory = new Category() { Name = categoryName };

                            if (!await _db.Categories.AnyAsync(e => e.Name == localCategory.Name))
                            {
                                await _db.Categories.AddAsync(localCategory);
                                await _db.SaveChangesAsync();
                                Console.WriteLine($"✅ Добавлена категория: {categoryName}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Ошибка при обработке категории: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка в альтернативном методе: {ex.Message}");
        }
    }

    // Старые методы для совместимости (можно удалить после тестирования)
    public List<IWebElement> ParseWomenBrands(ChromeDriver driver) => new List<IWebElement>();
    public List<IWebElement> ParseManBrands(ChromeDriver driver) => new List<IWebElement>();

}