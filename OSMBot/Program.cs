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
        static List<Job> jobs;
        static int maxTimesToDoJob = 50;
        static int maxDanger = 1;
        static int minExperience = 20;
        static void Main(string[] args)
        {

            jobs = new List<Job>();

            random = new Random();

            ChromeOptions options = new ChromeOptions();
            //options.AddArgument("--headless");

            options.AddExtension(@"D:\Bots\TheWest\OSMBot\bin\Debug\netcoreapp3.1\extension_4_11_0_0.crx");
            options.AddExtension(@"D:\Bots\TheWest\OSMBot\bin\Debug\netcoreapp3.1\extension_5_3_23_0.crx");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--log-level=3");
            driver = new ChromeDriver(options);
            js = (IJavaScriptExecutor)driver;

            InstallScripts();
            Login();
            jobs = GetFeatureJobs();
            Menu();
        }

        public static void Menu()
        {
            System.Console.WriteLine("Escolha uma opção");
            System.Console.WriteLine("1. Farmar EXP");
            System.Console.WriteLine("2. Farmar Comércio");
            System.Console.WriteLine("3. Atualizar Trabalhos");
            System.Console.WriteLine("9. Sair");
            var choise = Console.ReadLine();

            switch (choise)
            {
                case "1":
                    FarmExp();
                    break;
                case "2":
                    System.Console.WriteLine("Disponivel em breve!");
                    Menu();
                    break;
                case "3":
                    UpdateJobs();
                    Menu();
                    break;
                case "9":
                    Environment.Exit(0);
                    break;
                default:
                    System.Console.WriteLine("Opção inválida!");
                    Menu();
                    break;
            }
        }

        public static void FarmExp()
        {
            var nextJob = GetNextJob(jobs);
            System.Console.WriteLine("Presionar ESC para voltar ao menu");
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                try
                {
                    DoJob(nextJob);
                    //Check atual job and nearby before go to next job
                    nextJob = GetNextJob(jobs);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erro, ver ficheiro Log. A reinicar.");
                    System.IO.File.AppendAllTextAsync("log.txt", $"Erro {DateTime.Now}:{Environment.NewLine}{e.ToString()}{Environment.NewLine}");
                    Login();
                    FarmExp();
                }
            }
            Menu();
        }

        public static void InstallScripts()
        {
            System.Threading.Thread.Sleep(random.Next(3000, 4000));
            driver.Navigate().GoToUrl("https://greasyfork.org/en/scripts/7226-tw-gold-jobs-finder-more-languages");
            System.Threading.Thread.Sleep(random.Next(3000, 4000));
            driver.FindElement(By.LinkText("Install this script")).Click();
            System.Threading.Thread.Sleep(random.Next(2000, 3000));
            Console.WriteLine("Instale o Script");
            Console.ReadLine();
        }

        public static void DoJob(Job job)
        {
            Console.WriteLine(job.Name);
            Console.WriteLine(job.Distance);
            Console.WriteLine(job.Experience);
            //Equipar set velocidade

            //Ir para o trabalho

            //Equipar set XP
            //js.ExecuteScript("EquipManager.switchEquip(950)");
            while (job.TimesDone < maxTimesToDoJob)
            {
                if (GetPlayerEnergyPercentage() > 0)
                {
                    if (NumberOfTasksInQeue() < 9)
                    {
                        js.ExecuteScript($"TaskQueue.add(new TaskJob({job.Id}, {job.PosX}, {job.PosY}, 15))");
                        foreach (var jobInList in jobs.Where(j => j.Id == job.Id))
                        {
                            jobInList.TimesDone++;
                        }
                        System.Threading.Thread.Sleep(random.Next(14000, 15000));
                    }
                }
                else
                {
                    while (NumberOfTasksInQeue() > 0)
                    {
                        System.Threading.Thread.Sleep(random.Next(5000, 6000));
                    }
                    GoSleep();
                }
            }
            //js.ExecuteScript("TaskQueue.cancelAll()");
            //Menu();
        }

        public static void Login()
        {
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
            driver.FindElement(By.LinkText("Kansas")).Click();
            System.Threading.Thread.Sleep(random.Next(3000, 4000));


            while (!driver.FindElement(By.ClassName("ui-loader-wrap")).GetAttribute("style").Contains("none"))
            {
                System.Threading.Thread.Sleep(random.Next(500, 1000));
            }

            Console.WriteLine("Acabou Loading");
        }

        public static void UpdateJobs()
        {
            foreach (var job in jobs)
            {
                var coords = $"x:{job.PosX},y:{job.PosY}";
                js.ExecuteScript($"Map.JobHandler.openJob({job.Id},{coords})");

                var jobInfoWindow = driver.FindElement(By.ClassName($"wjob-{job.Id}"));
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

                job.Experience = Int32.Parse(experience);
                job.CanDo = canDo;
                job.Danger = danger;
                job.TimesDone = 0;

                jobInfoWindow.FindElement(By.ClassName("tw2gui_window_buttons_close")).Click();
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

                var experience = "0";
                var danger = 0;
                var canDo = false;

                if (!GoldJobRow.FindElement(By.ClassName("cell_3")).FindElement(By.ClassName("tw2gui_button")).GetAttribute("class").Contains("inactive"))
                {
                    js.ExecuteScript(JsAttribute);
                    var jobInfoWindow = driver.FindElement(By.ClassName($"wjob-{jobNumber}"));
                    while (!IsElementPresent(By.ClassName("job_rightSide")))
                    {
                        System.Threading.Thread.Sleep(random.Next(100, 200));
                    }
                    var rightSideInfoWindow = jobInfoWindow.FindElement(By.ClassName("job_rightSide"));


                    if (IsElementPresent(By.ClassName("job_jobtimes")))
                    {
                        danger = jobInfoWindow.FindElement(By.ClassName("job_dangerbox")).FindElement(By.ClassName("tw2gui_progresscircle")).FindElements(By.TagName("img")).Count;
                        experience = jobInfoWindow.FindElement(By.ClassName("job_durationbar_short")).FindElement(By.ClassName("job_value_experience")).Text;
                        canDo = true;
                    }

                    jobInfoWindow.FindElement(By.ClassName("tw2gui_window_buttons_close")).Click();
                }

                job.Id = Int32.Parse(jobNumber);
                job.PosX = Int32.Parse(jobPosX);
                job.PosY = Int32.Parse(JobPosY);
                job.CenterMap = CenterMap;
                job.Experience = Int32.Parse(experience);
                job.CanDo = canDo;
                job.Danger = danger;
                job.Name = js.ExecuteScript($"return JobList.getJobById({jobNumber}).name").ToString();
                job.Distance = 0;

                if (job.Experience >= minExperience && job.Danger <= maxDanger)
                {
                    featureJobs.Add(job);
                }

            }

            System.IO.File.AppendAllTextAsync("jobs.txt", $"Trabalhos {DateTime.Now}:{Environment.NewLine}{JsonConvert.SerializeObject(featureJobs, Formatting.Indented)}");
            return featureJobs;
        }

        public static Job GetNextJob(List<Job> jobs)
        {
            if (!jobs.Where(j => j.TimesDone < maxTimesToDoJob).Any())
            {
                UpdateJobs();
            }

            foreach (var job in jobs.Where(j => j.TimesDone < maxTimesToDoJob))
            {
                var distance = double.Parse(js.ExecuteScript($"return Character.calcWayTo({job.PosX}, {job.PosY})").ToString());
                job.Distance = distance;
            }
            return jobs.Where(j => j.TimesDone < maxTimesToDoJob).OrderBy(j => j.Distance).FirstOrDefault();
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
            //CityId = 2568
            //Character.homeTown - Info sobre a cidade - obter CityId para usar no Hotel
            //HotelWindow.open(cityId) - abrir janela hotel
            //HotelWindow.start("luxurious_apartment")

            //var rooms = ['cubby', 'bedroom', 'hotel_room', 'apartment', 'luxurious_apartment'];

            js.ExecuteScript("EquipManager.switchEquip(838)");
            js.ExecuteScript("HotelWindow.open(3158)");
            js.ExecuteScript("HotelWindow.start(\"cubby\")");
            while (GetPlayerEnergyPercentage() < 95)
            {
                System.Threading.Thread.Sleep(120000);
            }
            js.ExecuteScript("EquipManager.switchEquip(950)");
            js.ExecuteScript("TaskQueue.cancelAll()");
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
