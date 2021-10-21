using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TheWestBot
{

    class Job
    {
        public int Id { get; set; }
        public int PosY { get; set; }
        public int PosX { get; set; }
        public int Experience { get; set; }
        public int TimesDone { get; set; }
        public bool CanDo { get; set; }
        public string CenterMap { get; set; }
        public double Distance { get; set; }
        public int Danger { get; set; }

    }
    class Program
    {
        static IWebDriver driver;
        static IJavaScriptExecutor js;
        static Random random;
        static void Main(string[] args)
        {

            List<Job> jobs = new List<Job>();
            List<int> acceptedJobs = new List<int>() { 144, 120, 134, 113, 77, 119, 138, 118, 103, 57, 106, 72, 69, 80, 53, 126, 96, 84, 62, 59, 58, 52, 111, 107, 102, 109, 95, 71, 79, 63, 112, 101, 97, 83, 66, 48, 46, 45, 43, 90, 86, 51, 44, 98, 82, 64, 61, 105, 85, 78, 75 };

            random = new Random();

            ChromeOptions options = new ChromeOptions();
            //options.AddArgument("--headless");

            options.AddExtension(@"D:\Bots\TheWest\OSMBot\bin\Debug\netcoreapp3.1\extension_4_11_0_0.crx");
            options.AddExtension(@"D:\Bots\TheWest\OSMBot\bin\Debug\netcoreapp3.1\extension_5_3_23_0.crx");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--log-level=3");
            driver = new ChromeDriver(options);
            js = (IJavaScriptExecutor)driver;

            login();


            //login
            void login()
            {
                try
                {

                    System.Threading.Thread.Sleep(random.Next(3000, 4000));
                    driver.Navigate().GoToUrl("https://greasyfork.org/en/scripts/7226-tw-gold-jobs-finder-more-languages");
                    System.Threading.Thread.Sleep(random.Next(3000, 4000));
                    driver.FindElement(By.LinkText("Install this script")).Click();
                    System.Threading.Thread.Sleep(random.Next(2000, 3000));
                    Console.WriteLine("Instale o Script");
                    Console.ReadLine();

                    Console.WriteLine("--------------------");
                    Console.WriteLine("A navegar para The West");
                    driver.Navigate().GoToUrl("https://www.the-west.com.pt/");
                    System.Threading.Thread.Sleep(random.Next(2000, 3000));
                    driver.FindElement(By.Name("username")).Clear();
                    driver.FindElement(By.Name("username")).SendKeys("plarika");
                    driver.FindElement(By.Name("userpassword")).Clear();
                    driver.FindElement(By.Name("userpassword")).SendKeys("plarika1");
                    driver.FindElement(By.Name("userpassword")).SendKeys(Keys.Enter);
                    System.Threading.Thread.Sleep(random.Next(2000, 3000));
                    driver.FindElement(By.LinkText("Justice")).Click();
                    System.Threading.Thread.Sleep(random.Next(3000, 4000));


                    while (!driver.FindElement(By.ClassName("ui-loader-wrap")).GetAttribute("style").Contains("none"))
                    {
                        System.Threading.Thread.Sleep(random.Next(500, 1000));
                    }

                    Console.WriteLine("Acabou Loading");

                    while (!IsElementPresent(By.Id("GoldJobsmenu")))
                    {
                        System.Threading.Thread.Sleep(random.Next(500, 1000));
                    }

                    System.Threading.Thread.Sleep(random.Next(3000, 4000));
                    driver.FindElement(By.Id("GoldJobsmenu")).Click();
                    System.Threading.Thread.Sleep(random.Next(3000, 4000));
                    driver.FindElement(By.XPath("//div[@id='popup-container']/div[3]/div/ul/li/span")).Click();

                    Console.WriteLine("1. - " + js.ExecuteScript("return Character.health").ToString());
                    //Console.WriteLine("2. - " + GetDataFromJsFunction("Character.health"));

                    while (!IsElementPresent(By.ClassName("goldwindow")))
                    {
                        System.Threading.Thread.Sleep(random.Next(500, 1000));
                    }

                    var GoldWindow = driver.FindElement(By.ClassName("goldwindow"));
                    var GoldJobsPanel = GoldWindow.FindElement(By.ClassName("tw2gui_scrollpane_clipper_contentpane"));

                    var numberOfJobs = GoldJobsPanel.FindElements(By.ClassName("row")).Count;
                    for (int i = 0; i <= numberOfJobs - 1; i++)
                    {
                        var job = new Job();
                        var GoldJobRow = GoldJobsPanel.FindElement(By.ClassName("row_" + i));
                        var GoldJob = GoldJobRow.FindElement(By.ClassName("cell_0"));
                        var distance = GoldJobRow.FindElement(By.ClassName("distanceTime")).Text.Replace(":", "");
                        var JsAttribute = GoldJob.FindElement(By.ClassName("featured")).GetAttribute("onClick");
                        var CenterMap = GoldJob.FindElement(By.ClassName("centermap")).GetAttribute("onClick");
                        var jobNumber = JsAttribute.Split('(', ')')[1].Split('{', '}')[0].Replace(",", "");
                        string jobPosX = JsAttribute.Split('(', ')')[1].Split('{', '}')[1].Split(',')[0].Replace("x", "").Replace(":", "");
                        string JobPosY = JsAttribute.Split('(', ')')[1].Split('{', '}')[1].Split(',')[1].Replace("y", "").Replace(":", "");

                        js.ExecuteScript(JsAttribute);
                        var jobInfoWindow = driver.FindElement(By.ClassName($"wjob-{jobNumber}"));
                        while (!IsElementPresent(By.ClassName("job_rightSide")))
                        {
                            System.Threading.Thread.Sleep(random.Next(100, 200));
                        }
                        var rightSideInfoWindow = jobInfoWindow.FindElement(By.ClassName("job_rightSide"));
                        var experience = "0";
                        var danger = 0;
                        var canDo = false;

                        if (IsElementPresent(By.ClassName("job_jobtimes")))
                        {
                            danger = jobInfoWindow.FindElement(By.ClassName("job_dangerbox")).FindElement(By.ClassName("tw2gui_progresscircle")).FindElements(By.TagName("img")).Count;
                            experience = jobInfoWindow.FindElement(By.ClassName("job_durationbar_short")).FindElement(By.ClassName("job_value_experience")).Text;
                            canDo = true;
                        }

                        jobInfoWindow.FindElement(By.ClassName("tw2gui_window_buttons_close")).Click();

                        job.Id = Int32.Parse(jobNumber);
                        job.PosX = Int32.Parse(jobPosX);
                        job.PosY = Int32.Parse(JobPosY);
                        job.CenterMap = CenterMap;
                        job.Experience = Int32.Parse(experience);
                        job.CanDo = canDo;
                        job.Danger = danger;
                        job.Distance = double.Parse(js.ExecuteScript($"return Character.calcWayTo({jobPosX}, {JobPosY})").ToString());

                        //if (acceptedJobs.Contains(job.Id))
                        //{
                        //    jobs.Add(job);
                        //}

                        jobs.Add(job);
                    }
                    Console.WriteLine(JsonConvert.SerializeObject(jobs.Where(j => j.TimesDone <= 25 && j.Experience >= 40).OrderBy(j => j.Danger).ThenByDescending(j => j.Experience), Formatting.Indented));

                    Console.ReadLine();

                    foreach (var job in jobs.Where(j => j.TimesDone <= 25 && j.Experience >= 40).OrderBy(j => j.Danger).ThenBy(j => j.Distance))
                    {
                        //js.ExecuteScript(job.CenterMap);
                        //System.Threading.Thread.Sleep(random.Next(1500, 2000));
                        //js.ExecuteScript($"document.getElementsByClassName(\"posx-{job.PosX} posy-{job.PosY}\")[0].click()");
                        //System.Threading.Thread.Sleep(random.Next(1000, 1500));
                        while (job.TimesDone < 25)
                        {
                            if (NumberOfTasksInQeue() < 4)
                            {
                                //js.ExecuteScript($"document.getElementsByClassName(\"job-{job.Id}\")[0].getElementsByClassName(\"instantwork-short\")[0].click()");
                                js.ExecuteScript($"TaskQueue.add(new TaskJob({job.Id}, {job.PosX}, {job.PosY}, 15))");
                                job.TimesDone++;
                                System.Threading.Thread.Sleep(random.Next(15000, 17000));
                            }
                        }
                    }

                    foreach (var job in jobs.Where(j => j.TimesDone <= 25 && j.Experience < 40).OrderBy(j => j.Danger).ThenBy(j => j.Distance))
                    {
                        //js.ExecuteScript(job.CenterMap);
                        //System.Threading.Thread.Sleep(random.Next(1500, 2000));
                        //js.ExecuteScript($"document.getElementsByClassName(\"posx-{job.PosX} posy-{job.PosY}\")[0].click()");
                        //System.Threading.Thread.Sleep(random.Next(1000, 1500));
                        while (job.TimesDone < 25)
                        {
                            if (NumberOfTasksInQeue() < 4)
                            {
                                //js.ExecuteScript($"document.getElementsByClassName(\"job-{job.Id}\")[0].getElementsByClassName(\"instantwork-short\")[0].click()");
                                //TaskQueue.add(new TaskJob(11, 25669, 13977, 15));
                                js.ExecuteScript($"TaskQueue.add(new TaskJob({job.Id}, {job.PosX}, {job.PosY}, 15))");
                                job.TimesDone++;
                                System.Threading.Thread.Sleep(random.Next(15000, 17000));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erro, ver ficheiro Log");
                    System.IO.File.AppendAllTextAsync("log.txt", $"Erro {DateTime.Now}:{Environment.NewLine}{e.ToString()}");
                }
            }
        }

        public static int NumberOfTasksInQeue()
        {
            var i = 0;
            while (IsElementPresent(By.ClassName($"task-queuePos-{i}")))
            {
                i++;
            }
            return i;
        }

        public static string GetDataFromJsFunction(string jsFunction)
        {
            System.Threading.Thread.Sleep(random.Next(100, 200));
            js.ExecuteScript($"document.getElementsByTagName(\"body\")[0].dataset.content = {jsFunction};");
            System.Threading.Thread.Sleep(random.Next(100, 200));
            return driver.FindElement(By.TagName("body")).GetAttribute("content");
        }

        public static void GoSleep()
        {

        }

        public static int GetPlayerEnergyPercentage()
        {
            var currentEnergy = Int32.Parse((string)js.ExecuteScript("Character.energy"));
            var maxEnergy = Int32.Parse((string)js.ExecuteScript("Character.maxEnergy"));
            return (currentEnergy / maxEnergy) * 100;
        }

        public static int GetPlayerHealthPercentage()
        {
            var currentHealth = Int32.Parse((string)js.ExecuteScript("Character.health"));
            var maxtHealth = Int32.Parse((string)js.ExecuteScript("Character.maxHealth"));
            return (currentHealth / maxtHealth) * 100;
        }

        public static bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
