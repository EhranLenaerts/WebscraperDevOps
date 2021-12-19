using System;
using System.Text;
using System.Threading;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace WebScraping
{
    class Youtube
    {
        public static void Yt()
        {
            //Setting up the CSV parameters "C:/Users/ehran/source/repos/WebScraping2/youtube.csv"
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/youtube.csv";
            string seperator = ";";
            StringBuilder stringOutput = new StringBuilder();
            stringOutput.AppendLine("link;title;uploader;views");
            //Preparing query data
            Console.WriteLine("What would you like to find on youtube?");
            String UserInput = Console.ReadLine();
            //Setting up driver options
            ChromeOptions options = new ChromeOptions();
            var chromeDriver = ChromeDriverService.CreateDefaultService();
            chromeDriver.SuppressInitialDiagnosticInformation = true;
            options.AddArgument("--log-level=3");
            //Create the reference for the browser with options
            IWebDriver driver = new ChromeDriver(chromeDriver, options);
            //Navigating to youtube
            driver.Navigate().GoToUrl("https://www.youtube.com/");
            Thread.Sleep(500);
            //Google sometimes asks to agree with ToS, this does that
            try
            {
                IWebElement ele3 = driver.FindElement(By.XPath("//*[text()='I agree']"));
                ele3.Click();
            }
            catch
            { }
            //Finding the search input and sending the query
            Thread.Sleep(1000);
            IWebElement ele = driver.FindElement(By.Name("search_query"));
            ele.SendKeys(UserInput);
            ele.SendKeys(Keys.Enter);
            Thread.Sleep(2500);
            //Filtering the videos for latest uploads
            driver.FindElement(By.XPath("//*[text()='Filters']")).Click();
            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//*[@id='label']/yt-formatted-string")).Click();
            Thread.Sleep(2500);
            //Getting all of the videos in the current list
            var videos = driver.FindElements(By.TagName("ytd-video-renderer"));
            //This for loop keeps going until it has cycled through 5 videos
            for (int i = 0; i < 5; i++)
            {
                //Appending all of the required info to the base string
                String toAppend = "";
                toAppend = toAppend + string.Join(seperator, videos[i].FindElement(By.TagName("a")).GetAttribute("href"));
                toAppend = toAppend + ";" + string.Join(seperator, videos[i].FindElement(By.TagName("h3")).Text);
                toAppend = toAppend + ";" + string.Join(seperator, videos[i].FindElement(By.Id("channel-info")).Text);
                String Views = videos[i].FindElement(By.Id("metadata-line")).FindElement(By.XPath("./span[1]")).Text;
                toAppend = toAppend + ";" + string.Join(seperator, Views);
                stringOutput.AppendLine(toAppend);
            };
            //Writing the base string to our CSV file
            File.WriteAllText(filePath, stringOutput.ToString());
            //Closing the driver
            driver.Close();
        }
    }
}
