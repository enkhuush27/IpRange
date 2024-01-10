using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;

namespace IpRange.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class IPRangeController : ControllerBase
  {
    const string URL = "https://lite.ip2location.com/mongolia-ip-address-ranges";

    [HttpGet]
    public IActionResult GetIPRanges()
    {
      try
      {
        var driver = ConnectURL(URL);
        var scrapedData = DataScrap(driver);
        driver.Quit();

        // Returning the scraped data directly as the API response
        return Ok(scrapedData);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Error: {ex.Message}");
      }
    }

    private IWebDriver ConnectURL(string url)
    {
      var options = new ChromeOptions();
      options.AddArgument("--headless");
      var driver = new ChromeDriver(options);
      driver.Navigate().GoToUrl(url);
      return driver;
    }

    private string DataScrap(IWebDriver driver)
    {
      var dataHtml = driver.FindElement(By.XPath("//table[@id='ip-address']"));
      var beginIPAddress = dataHtml.FindElements(By.ClassName("sorting_1"));
      var endIPAddress = dataHtml.FindElements(By.XPath(".//tbody/tr/td[2]"));

      var ipsRanges = new List<string>();
      for (var i = 0; i < beginIPAddress.Count; i++)
        ipsRanges.Add($"{beginIPAddress[i].Text.Trim()}-{endIPAddress[i].Text.Trim()}");

      // Returning the scraped data as a string
      return string.Join(",", ipsRanges);
    }
  }
}
