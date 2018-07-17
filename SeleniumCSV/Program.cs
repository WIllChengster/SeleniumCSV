using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumCSV
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver;
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("http://localhost:62625/ReaderPlus/Demo");
            IWebElement fileInput = driver.FindElement(By.CssSelector("#apdf-add-file-demo"));
            fileInput.SendKeys("C:\\Users\\Administrator\\Desktop\\testFilesPDF\\3.pdf");
        }
        public static void CreateForm(IWebDriver driver, int page)
        {
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));
            IWebElement pdfPages = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector($".apdf-sidebar-toc-image[data-page='{page}']")));

            pdfPages.Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".apdf-main-menu >ul > li:nth-child(5) > a"))).Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div#EditForm > nav > ul > li:first-child > a"))).Click();
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#Fields ul > li:nth-child(1) > .apdf-option.apdf-icon"))).Click();

            IWebElement pdf = driver.FindElement(By.CssSelector($"div#form-fields-container[data-page='{page}']"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            Scroll(driver, pdfPages);

            string script = @"
                    var pageLocation = $("".pdf-page[data-page='" + $"{page}" + @"']"").offset();


                    var down = jQuery.Event(""mousedown"", {
                        which: 1,
                        pageX: pageLocation.left + 50,
                        pageY: pageLocation.top + 50,
    
                    })
                    var move = jQuery.Event(""mousemove"",{
                        whch: 1,
                        pageX: pageLocation.left + 200,
                        pageY: pageLocation.top + 200,

                    })

                    var up = jQuery.Event(""mouseup"",{
                        whch: 1,
                        pageX: pageLocation.left + 200,
                        pageY: pageLocation.top + 200,

                    })

                    $("".pdf-page[data-page='" + $"{page}" + @"']"").trigger(down);
                    setTimeout( 
                        function(){
                            $("".pdf-page[data-page='" + $"{page}" + @"']"").trigger(move)
                            $("".pdf-page[data-page='" + $"{page}" + @"']"").trigger(up)
                        }, 500);
                    ";
            js.ExecuteScript(script);
            System.Threading.Thread.Sleep(650);
        }

        public static void Scroll(IWebDriver driver, IWebElement selector)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            js.ExecuteScript("arguments[0].scrollIntoView();", selector);
        }

    }
}
