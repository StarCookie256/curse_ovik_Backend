using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace PerfumeryBackend.ParserLayer.Interfaces;

public interface IParserService
{
    public Task ParseData();
    public void ParseCategories(ChromeDriver driver);
    public List<IWebElement> ParseManBrands(ChromeDriver driver);
    public List<IWebElement> ParseWomenBrands(ChromeDriver driver);
}
