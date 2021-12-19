using System;
using System.Text;
using System.Threading;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace WebScraping3
{
    class Twitter
    {
        static string Csv(IWebElement Title, IWebElement article)
        {
            string seperator = ";";
            String toAppend = "";
            try
            {
                //Finding the required elements
                IWebElement nameTitle = Title.FindElement(By.TagName("a"));
                IWebElement Comment = article.FindElement(By.XPath(".//div[@data-testid='reply']"));
                IWebElement Retweet = article.FindElement(By.XPath(".//div[@data-testid='retweet']"));
                IWebElement Like = article.FindElement(By.XPath(".//div[@data-testid='like']"));
                IWebElement Tweet = article.FindElement(By.XPath(".//div[@lang='en']"));
                //Appending all the elements to our base string
                toAppend = $"{toAppend}{string.Join(seperator, $"{nameTitle.GetAttribute("href")}")}";
                toAppend = $"{toAppend};{string.Join(seperator, article.FindElement(By.ClassName("r-1d09ksm")).FindElement(By.XPath("./a")).GetAttribute("href"))}";
                toAppend = $"{toAppend};{string.Join(seperator, Tweet.Text)}";
                toAppend = $"{toAppend};{string.Join(seperator, Comment.Text)}";
                toAppend = $"{toAppend};{string.Join(seperator, Like.Text)}";
                toAppend = $"{toAppend};{string.Join(seperator, Retweet.Text)}";
            }
            catch
            { }

            return toAppend;
        }
        public static void Tweets()
        {
            //Setting up the beginning of our CSV file and queries
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/twitter.csv";
            StringBuilder stringOutput = new StringBuilder();
            stringOutput.AppendLine("User;Link;Tweet;Comments;Likes;Retweets");
            Console.WriteLine("Search query");
            string query = (Console.ReadLine());
            Console.WriteLine("How many tweets would you like to scrape? (Numbers only)");
            var amount = int.Parse(Console.ReadLine());
            //Setting driver parameters
            var chromeDriver = ChromeDriverService.CreateDefaultService();
            chromeDriver.SuppressInitialDiagnosticInformation = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--log-level=3");
            //Create the reference for the browser using previously set parameters
            IWebDriver driver = new ChromeDriver(chromeDriver, options);
            //Create the reference for Javascript to allow for scrolling
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            try
            {
                //Navigating to Twitter
                driver.Navigate().GoToUrl("https://twitter.com/explore");
                Thread.Sleep(5000);
                //In case of cookies at the bottom this try block will close it
                try
                {
                    driver.FindElement(By.XPath("//*[text()='Close']")).Click();
                }
                catch { };
                //Finding the search input and sending our query
                IWebElement search = driver.FindElement(By.XPath("//input[@aria-label='Search query']"));
                search.SendKeys(query);
                search.SendKeys(Keys.Enter);
                Thread.Sleep(2500);
                //Getting the current in range articles
                var articles = driver.FindElements(By.XPath("//article[@tabindex='0']"));
                //As long as we don't have the amount of tweets that we want the loops will continue
                var C = 0;
                while (C < amount)
                {
                    foreach (var article in articles)
                    {
                        //There are two types of articles, this try/catch block gets both of them
                        try
                        {
                            IWebElement Title = article.FindElement(By.ClassName("r-1777fci"));
                            stringOutput.AppendLine(Twitter.Csv(Title, article));
                        }
                        catch
                        {
                            IWebElement Title = article.FindElement(By.ClassName("r-zl2h9q"));
                            Title = Title.FindElement(By.ClassName("r-1wbh5a2"));
                            stringOutput.AppendLine(Twitter.Csv(Title, article));
                        }
                        C++;
                    }
                    //After each article group the program scrolls down 3500 pixels to load in a new article group and repeat
                    js.ExecuteScript("window.scrollBy(0, 3500);");
                    Thread.Sleep(1500);
                    articles = driver.FindElements(By.XPath("//article[@tabindex='0']"));
                }
                //Writing the base string into the CSV file
                File.WriteAllText(filePath, stringOutput.ToString());
            }
            catch
            {
                Console.WriteLine("Something went wrong.");
            }
            //At the end of the program the driver closes itself
            driver.Close();
        }

    }
}
