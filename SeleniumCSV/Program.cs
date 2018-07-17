using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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



            fileInput.SendKeys( Directory.GetCurrentDirectory() + @"\3.pdf");

            CreateForm(driver, 1);

            pointer(driver);



            string[] numbersArr = ReadCSV_GetValues();

            //iterate of numbersArr and send the values of each index into the formfield
            for (int i = 0; i < numbersArr.Count(); i++)
            {
                driver.FindElement(By.CssSelector(".formfield")).SendKeys(numbersArr[i]);
            }
            
        }

        //Reads the CSV file and then returns an array of values
        public static string[] ReadCSV_GetValues()
        {
            StreamReader sr = new StreamReader(File.OpenRead(Directory.GetCurrentDirectory() + @"\numbers.csv"));
            List<string> numbers = new List<string>();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                numbers.Add(line);
            }
            string[] numbersArr = numbers.ToArray();

            return numbersArr;
        }

        //switches to pointer
        public static void pointer(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 5));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("a.apdf-option[title*='Pointer']")));
        }

        //Creates the form
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

            //I use jQuery here because Selenium's action builder is too fast for ReaderPlus to process
            //so it starts to bug out. jQuery has more realistic user input allowing ReaderPlus to operate as intended.
            string script = @"
                    var pageLocation = $("".pdf-page[data-page='" + page + @"']"").offset();


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

                    $("".pdf-page[data-page='" + page + @"']"").trigger(down);
                    setTimeout( 
                        function(){
                            $("".pdf-page[data-page='" + page  + @"']"").trigger(move)
                            $("".pdf-page[data-page='" + page + @"']"").trigger(up)
                        }, 500);
                    ";
            js.ExecuteScript(script);
            System.Threading.Thread.Sleep(650);
        }

        //Scrolls into whatever page you're looking for.
        //Without this method, looking for a sidebar page thumbnail
        //that is hidden from view will bug and crash.
        public static void Scroll(IWebDriver driver, IWebElement selector)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            js.ExecuteScript("arguments[0].scrollIntoView();", selector);
        }

    }
}
