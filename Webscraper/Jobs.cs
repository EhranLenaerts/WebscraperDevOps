using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
namespace WebScraping2
{
    class IndeedJob
    {
        public static void Job()
        {
            //Setting driver parameters
            var chromeDriver = ChromeDriverService.CreateDefaultService(".\\");
            chromeDriver.SuppressInitialDiagnosticInformation = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--log-level=3");
            //create the reference for the browser using the options we set beforehand
            IWebDriver driver = new ChromeDriver(chromeDriver, options);
            //Create the reference for Javascript to allow for scrolling
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //Setting up the beginning of our CSV file and queries
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/jobs.csv";
            string seperator = ";";
            StringBuilder stringOutput = new StringBuilder();
            stringOutput.AppendLine("Title;company;location;link");
            Console.WriteLine("What job are you looking for?");
            String query = Console.ReadLine();
            try
            {
                //Navigating to be.indeed.com
                driver.Navigate().GoToUrl("https://be.indeed.com/");
                Thread.Sleep(1000);
                //Finding the search input and putting in our query
                IWebElement ele1 = driver.FindElement(By.Id("text-input-what"));
                ele1.SendKeys(query);
                ele1.SendKeys(Keys.Enter);
                Thread.Sleep(1500);
                //Finding the filter and filtering for jobs posted within 3 days
                driver.FindElement(By.Id("filter-dateposted")).Click();
                driver.FindElement(By.Id("filter-dateposted-menu")).FindElement(By.XPath("./li[2]")).Click();
                Thread.Sleep(1000);
                //After filtering a popup shows, this try block closes it
                try
                {
                    driver.FindElement(By.Id("popover-x")).Click();
                }
                catch
                { }
                //The loop will continue going until all of the chosen jobs have been written into the CSV file
                int counter = 0;
                var T = true;
                while (T == true)
                {
                    //Getting all of the jobs on the current page
                    var eleJobs = driver.FindElements(By.ClassName("result"));
                    foreach (var eleJob in eleJobs)
                    {
                        //Appending all of the required information
                        string toAppend = "";
                        toAppend = toAppend + string.Join(seperator, eleJob.FindElement(By.ClassName("jobTitle")).FindElement(By.XPath("./span")).Text);
                        toAppend = toAppend + ";" + string.Join(seperator, eleJob.FindElement(By.ClassName("companyName")).Text);
                        toAppend = toAppend + ";" + string.Join(seperator, eleJob.FindElement(By.ClassName("companyLocation")).Text);
                        toAppend = toAppend + ";" + string.Join(seperator, eleJob.GetAttribute("href"));
                        stringOutput.AppendLine(toAppend);
                        counter++;
                        //When all of the jobs have been cycled through on the current page it will try and find the next
                        //page button, if it cannot the program will end since all pages will have been exhausted
                        if (counter == eleJobs.Count())
                        {
                            //Sometimes cookies popup, this closes it
                            try
                            {
                                IWebElement Cookies = driver.FindElement(By.Id("onetrust-accept-btn-handler"));
                                Cookies.Click();
                            }
                            catch
                            {
                                Console.WriteLine("No cookies");
                            }
                            //Scrolling down to reach the next page button
                            try
                            {
                                js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                                Thread.Sleep(1000);
                                driver.FindElement(By.ClassName("pagination-list")).FindElement(By.XPath("./li[last()]/a/span/span")).Click();
                                Thread.Sleep(1500);
                                counter = 0;
                            }
                            //When no next page button is found the variable T is turned into false and the while loop stops
                            catch (Exception ex)
                            {
                                T = false;
                                Console.WriteLine(eleJobs.Count());
                            }
                        }
                    }
                }
                //Writing the base string to the CSV file and closing the driver
                File.WriteAllText(filePath, stringOutput.ToString());
                driver.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}