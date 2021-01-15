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

    }
    class Program
    {
        static IWebDriver driver;
        static void Main(string[] args)
        {

            List<Job> jobs = new List<Job>();
            List<int> acceptedJobs = new List<int>();
            acceptedJobs.Add(103);

            var random = new Random();

            ChromeOptions options = new ChromeOptions();
            //options.AddArgument("--headless");

            options.AddExtension(@"D:\Bots\TheWest\OSMBot\bin\Debug\netcoreapp3.1\extension_4_11_0_0.crx");
            options.AddExtension(@"D:\Bots\TheWest\OSMBot\bin\Debug\netcoreapp3.1\extension_5_3_23_0.crx");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--log-level=3");
            driver = new ChromeDriver(options);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

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
                        var JsAttribute = GoldJob.FindElement(By.ClassName("featured")).GetAttribute("onClick");
                        var CenterMap = GoldJob.FindElement(By.ClassName("centermap")).GetAttribute("onClick");
                        var jobNumber = JsAttribute.Split('(', ')')[1].Split('{', '}')[0].Replace(",", "");
                        string jobPosX = JsAttribute.Split('(', ')')[1].Split('{', '}')[1].Split(',')[0].Replace("x", "").Replace(":", "");
                        string JobPosY = JsAttribute.Split('(', ')')[1].Split('{', '}')[1].Split(',')[1].Replace("y", "").Replace(":", "");

                        job.Id = Int32.Parse(jobNumber);
                        job.PosX = Int32.Parse(jobPosX);
                        job.PosY = Int32.Parse(JobPosY);
                        job.CenterMap = CenterMap;

                        if (acceptedJobs.Contains(job.Id))
                        {
                            jobs.Add(job);
                        }
                    }

                    foreach (var job in jobs)
                    {
                        js.ExecuteScript('"' + job.CenterMap + '"');
                        js.ExecuteScript("javascript:Map.center(25237,15126);");
                        System.Threading.Thread.Sleep(random.Next(1500, 2000));
                        //string openJob = "document.getElementsByClassName(\"posx-25237 posy-15126\")[0].click()";
                        //Console.WriteLine(openJob);
                        js.ExecuteScript("document.getElementsByClassName(\"posx-25237 posy-15126\")[0].click()");
                        System.Threading.Thread.Sleep(random.Next(1000, 1500));
                        while (job.TimesDone < 25)
                        {
                            js.ExecuteScript(@"""document.getElementsByClassName(""job-103"")[0].getElementsByClassName(""instantwork-short"")[0].click()""");
                            job.TimesDone++;
                            System.Threading.Thread.Sleep(random.Next(15000, 16000));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erro, ver ficheiro Log");
                    ErrorLogging(e);
                }
            }

            //void verAnuncio()
            //{
            //    try
            //    {
            //        Console.WriteLine("Abrindo Anuncios");
            //        Console.WriteLine("--------------------------");
            //        driver.Navigate().GoToUrl("https://en.onlinesoccermanager.com/Career");
            //        System.Threading.Thread.Sleep(random.Next(1000, 3000));
            //        driver.FindElement(By.XPath("//div[@id='balances']/div/div[3]")).Click();
            //        System.Threading.Thread.Sleep(random.Next(3000, 5000));
            //        driver.FindElement(By.XPath("//div[@id='product-category-free']/div[2]/div/div/div/div/div")).Click();
            //        System.Threading.Thread.Sleep(random.Next(3000, 5000));
            //        try
            //        {
            //            var caixaSemAnuncios = driver.FindElement(By.XPath("//div[@id='modal-dialog-alert']/div[4]/div/div/div/div[2]/div/div/p"));
            //            var btnSemAnuncios = driver.FindElement(By.XPath("//div[@type='button']"));

            //            btnSemAnuncios.Click();
            //            Console.WriteLine("Sem mais anuncios para ver");
            //            Console.WriteLine("--------------------------");
            //            Console.WriteLine("Esperando 1 hora");
            //            Console.WriteLine("--------------------------");
            //            Console.WriteLine("Proximo premio disponivel as : {0}", DateTime.Now.AddHours(1));

            //            for (int i = 6; i >= 1; i--)
            //            {
            //                var tempoEspera = 600000;
            //                Console.WriteLine("Esperando {0} minutos as {1}", i * 10, DateTime.Now);
            //                System.Threading.Thread.Sleep(tempoEspera);
            //            }
            //            verAnuncio();
            //        }
            //        catch
            //        {
            //            Console.WriteLine("A ver anuncio");
            //            Console.WriteLine("--------------------------");
            //            System.Threading.Thread.Sleep(1000 * 40);
            //            Console.WriteLine("A ver proximo Anuncio");
            //            Console.WriteLine("--------------------------");
            //            verAnuncio();
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("Erro, ver ficheiro Log");
            //        ErrorLogging(e);
            //        login();
            //    }
            //}

            //void verQuantidadeDeMoedas()
            //{
            //    try
            //    {
            //        System.Threading.Thread.Sleep(random.Next(2500, 3500));
            //        var moedas = driver.FindElement(By.XPath("//div[@id='balances']/div/div[2]/span")).Text;
            //        Console.WriteLine("Quantidade de moedas : " + moedas);
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("Impossivel ver as moedas");
            //        ErrorLogging(e);
            //    }
            //}
        }
        public static void ErrorLogging(Exception ex)
        {
            string strPath = @"C:\Users\tiago\Desktop\TheWestBotLog.txt";
            if (!File.Exists(strPath))
            {
                File.Create(strPath).Dispose();
            }
            using (StreamWriter sw = File.AppendText(strPath))
            {
                Console.WriteLine("Erro reportado");
                sw.WriteLine("=============Error Logging ===========");
                sw.WriteLine("===========Start============= " + DateTime.Now);
                sw.WriteLine("Error Message: " + ex.Message);
                sw.WriteLine("Stack Trace: " + ex.StackTrace);
                sw.WriteLine("===========End============= " + DateTime.Now);
            }
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
