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
        public string Name { get; set; }

    }
    class Program
    {
        static IWebDriver driver;
        static IJavaScriptExecutor js;
        static Random random;
        static void Main(string[] args)
        {

            List<Job> jobs = new List<Job>();

            random = new Random();

            ChromeOptions options = new ChromeOptions();
            //options.AddArgument("--headless");

            options.AddExtension(@"C:\Bots\TheWest\OSMBot\bin\Debug\netcoreapp3.1\extension_4_11_0_0.crx");
            options.AddExtension(@"C:\Bots\TheWest\OSMBot\bin\Debug\netcoreapp3.1\extension_5_3_23_0.crx");
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

                    jobs = GetFeatureJobs();

                    var nextJob = GetNextJob(jobs);

                    while (true)
                    {
                        Console.WriteLine(nextJob.Name);
                        Console.WriteLine(nextJob.Distance);
                        Console.WriteLine(nextJob.Experience);
                        Console.ReadLine();
                        while (nextJob.TimesDone < 25)
                        {
                            if (GetPlayerEnergyPercentage() > 0)
                            {
                                if (NumberOfTasksInQeue() < 4)
                                {
                                    js.ExecuteScript($"TaskQueue.add(new TaskJob({nextJob.Id}, {nextJob.PosX}, {nextJob.PosY}, 15))");
                                    nextJob.TimesDone++;
                                    System.Threading.Thread.Sleep(random.Next(15000, 17000));
                                }
                            }
                            else
                            {
                                GoSleep();
                            }
                        }
                        nextJob = GetNextJob(jobs);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erro, ver ficheiro Log");
                    System.IO.File.AppendAllTextAsync("log.txt", $"Erro {DateTime.Now}:{Environment.NewLine}{e.ToString()}");
                }
            }
        }

        public static List<Job> GetFeatureJobs()
        {
            var featureJobs = new List<Job>();

            while (!IsElementPresent(By.Id("GoldJobsmenu")))
            {
                System.Threading.Thread.Sleep(random.Next(500, 1000));
            }

            System.Threading.Thread.Sleep(random.Next(3000, 4000));
            driver.FindElement(By.Id("GoldJobsmenu")).Click();
            System.Threading.Thread.Sleep(random.Next(3000, 4000));
            driver.FindElement(By.XPath("//div[@id='popup-container']/div[3]/div/ul/li/span")).Click();

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
                job.Name = js.ExecuteScript($"return JobList.getJobById({jobNumber}).name").ToString();
                //job.Distance = double.Parse(js.ExecuteScript($"return Character.calcWayTo({jobPosX}, {JobPosY})").ToString());
                job.Distance = 0;

                if (job.Experience >= 40 && job.Danger <= 2)
                {
                    featureJobs.Add(job);
                }

            }

            return featureJobs;
        }

        public static Job GetNextJob(List<Job> jobs)
        {
            foreach (var job in jobs.Where(j => j.TimesDone <= 25))
            {
                var distance = double.Parse(js.ExecuteScript($"return Character.calcWayTo({job.PosX}, {job.PosY})").ToString());
                Console.WriteLine("distancia : " + distance);
                job.Distance = distance;
            }
            return jobs.OrderBy(j => j.Distance).FirstOrDefault();
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
            driver.FindElement(By.ClassName("city")).Click();
            while (GetPlayerEnergyPercentage() < 95)
            {
                System.Threading.Thread.Sleep(120000);
            }
        }

        public static int GetPlayerEnergyPercentage()
        {
            var currentEnergy = Int32.Parse(js.ExecuteScript("return Character.energy").ToString());
            //var maxEnergy = Int32.Parse(js.ExecuteScript("return Character.maxEnergy").ToString());
            //Console.WriteLine("current: " + currentEnergy);
            //Console.WriteLine("max: "+maxEnergy);
            //Console.WriteLine((currentEnergy / maxEnergy) * 100);
            //Console.WriteLine(currentEnergy / maxEnergy);
            return currentEnergy;
        }

        public static int GetPlayerHealthPercentage()
        {
            var currentHealth = Int32.Parse(js.ExecuteScript("return Character.health").ToString());
            var maxtHealth = Int32.Parse(js.ExecuteScript("return Character.maxHealth").ToString());
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
