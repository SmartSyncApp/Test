using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
//using System.Threading;
using System.Threading.Tasks;

namespace C_MOVE_RSP_Read
{
    class Program
    {
        static void Main(string[] args)
        {
            FindResponse();
            //deleteFiles();
        }

        private static void FindResponse()
        {
            try
            {
                string startdate = "08-17-2016";
                string endDate = "08-30-2016";

                DateTime dtStart = DateTime.Parse(startdate);
                DateTime dtEnd = DateTime.Parse(endDate);

                int totalDays = (int)(dtEnd - dtStart).TotalDays + 1;

                for (int i = 0; i < totalDays; i++)
                {
                    File.AppendAllText(ConfigurationManager.AppSettings["logPath"], "======================================================================================================================================================" + Environment.NewLine);
                    string currentDirectory = dtEnd.ToString("yyyyMMdd");

                    string[] logFiles = Directory.GetFiles(ConfigurationManager.AppSettings["archivePath"] + currentDirectory + @"\Log_" + ConfigurationManager.AppSettings["destinationAET"]);
                    string completeLog = string.Empty;

                    foreach (var item in logFiles)
                    {
                        completeLog += File.ReadAllText(item);
                    }

                    string[] logArray = completeLog.Split('\n');

                    int movingCount = logArray.Where(z => z.ToLower().StartsWith("moving")).Count();
                    Console.WriteLine(dtEnd);
                    Console.WriteLine(movingCount);

                    string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["archivePath"] + currentDirectory + @"\RSP_" + ConfigurationManager.AppSettings["destinationAET"]);

                    StringBuilder completeResponse = new StringBuilder();

                    foreach (var item in files)
                    {
                        completeResponse.Append(File.ReadAllText(item));
                    }

                    //string[] responseArray = completeResponse.Split('\n');

                    int successCount = 0; int failureCount = 0; int routerOfflineCount = 0;
                    Get("0H", completeResponse.ToString(), dtEnd, out successCount);
                    Get("b000H", completeResponse.ToString(), dtEnd, out failureCount);
                    Get("c801H", completeResponse.ToString(), dtEnd, out routerOfflineCount);

                    bool isErrored = false;
                    if (movingCount != successCount + failureCount)
                    {
                        isErrored = true;
                    }

                    if (ConfigurationManager.AppSettings["moveLevel"] == "SERIES")
                    {
                        File.AppendAllText(ConfigurationManager.AppSettings["logPath"], dtEnd.ToString() + '\t' + "Total series to be moved =" + movingCount + '\t' + "SuccessCount =" + successCount + '\t' + "FailureCount =" + failureCount + '\t' + " Is Errored = " + isErrored + Environment.NewLine);
                    }
                    else
                    {
                        File.AppendAllText(ConfigurationManager.AppSettings["logPath"], dtEnd.ToString() + '\t' + "Total study to be moved =" + movingCount + '\t' + "SuccessCount =" + successCount + '\t' + "FailureCount =" + failureCount + '\t' + " Is Errored = " + isErrored + Environment.NewLine);
                    }

                    //GetLogfiles(ConfigurationManager.AppSettings["archivePath"] + currentDirectory + @"\Log_" + ConfigurationManager.AppSettings["destinationAET"]);

                    dtEnd = dtEnd.AddDays(-1);
                }
                while (true)
                {

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static List<string> Get(string status, string completeResponse, DateTime dtEnd, out int count)
        {
            int startSearch = 0;
            //string status = "0H";//b000H //0H
            count = 0;
            List<string> lstStatus = new List<string>();

            using (StringReader reader = new StringReader(completeResponse))
            {
                string item;
                while ((item = reader.ReadLine()) != null)
                {
                    if (item.Contains("status=" + status))
                    {
                        count += 1;
                        int failurePosition = 0;
                        if (startSearch == 0)
                        {
                            failurePosition = completeResponse.IndexOf("status=" + status, 0);
                            startSearch = failurePosition + ("status=" + status).Length;
                        }
                        else
                        {
                            failurePosition = completeResponse.IndexOf("status=" + status, startSearch);
                            startSearch = failurePosition + ("status=" + status).Length;
                        }

                        //SUID
                        int SUIDStartIndex = completeResponse.LastIndexOf("-m 0020000D=", failurePosition) + "-m 0020000D=".Length;
                        int SUIDEndIndex = completeResponse.IndexOf(" ", SUIDStartIndex);
                        string SUID = completeResponse.Substring(SUIDStartIndex, SUIDEndIndex - SUIDStartIndex);

                        string SEUID = string.Empty;
                        if (ConfigurationManager.AppSettings["moveLevel"] == "SERIES")
                        {
                            //SEUID
                            int SEUIDStartIndex = completeResponse.LastIndexOf("-m 0020000E=", failurePosition) + "-m 0020000E=".Length;
                            int SEUIDEndIndex = completeResponse.IndexOf(" ", SEUIDStartIndex);
                            SEUID = completeResponse.Substring(SEUIDStartIndex, SEUIDEndIndex - SEUIDStartIndex);
                        }

                        //if (status == "b000H")
                        if (status != "0H")
                        {
                            File.AppendAllText(ConfigurationManager.AppSettings["logPath"], dtEnd.ToString() + '\t' + SUID + '\t' + SEUID + '\t' + status + Environment.NewLine);

                        }
                    }
                    if (item.Contains("status="))
                    {
                        string stat = item.Substring(item.IndexOf("status=") + "status=".Length);
                        if (!lstStatus.Contains(stat))
                        {
                            lstStatus.Add(stat);
                        }
                    }
                }
            }
            
            //foreach (var item in completeResponse.Split('\n'))
            //{
                
            //}
            return lstStatus;
        }

        private static void deleteFiles()
        {
            string startdate = "01-06-2016";
            string endDate = "01-29-2016";

            DateTime dtStart = DateTime.Parse(startdate);
            DateTime dtEnd = DateTime.Parse(endDate);

            int totalDays = (int)(dtEnd - dtStart).TotalDays + 1;

            for (int i = 0; i < totalDays; i++)
            {
                string currentDirectory = @"C:\Out\Archive\" + dtEnd.ToString("yyyyMMdd");

                Console.WriteLine("Deleting files for " + dtEnd);

                if (Directory.Exists(currentDirectory))
                {
                    Directory.Delete(currentDirectory, true);
                }

                dtEnd = dtEnd.AddDays(-1);
            }
        }

        private static void FindResponseByPath()
        {
            try
            {
                string archivePath = ConfigurationManager.AppSettings["archivePath"];
                string[] directoryList = Directory.GetDirectories(archivePath);

                for (int i = 0; i < directoryList.Count(); i++)
                {
                    File.AppendAllText(ConfigurationManager.AppSettings["logPath"], "======================================================================================================================================================" + Environment.NewLine);

                    string directoryName = directoryList[i].Split('\\')[directoryList[i].Split('\\').Length - 1];

                    string currentDirectory = directoryName;

                    DateTime dtEnd = DateTime.Parse(currentDirectory.Insert(4, "-").Insert(7, "-"));

                    string[] logFiles = Directory.GetFiles(ConfigurationManager.AppSettings["archivePath"] + currentDirectory + @"\Log_" + ConfigurationManager.AppSettings["destinationAET"]);
                    string completeLog = string.Empty;

                    foreach (var item in logFiles)
                    {
                        completeLog += File.ReadAllText(item);
                    }

                    string[] logArray = completeLog.Split('\n');

                    int movingCount = logArray.Where(z => z.ToLower().StartsWith("moving")).Count();
                    Console.WriteLine(dtEnd);
                    Console.WriteLine(movingCount);

                    string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["archivePath"] + currentDirectory + @"\RSP_" + ConfigurationManager.AppSettings["destinationAET"]);

                    string completeResponse = string.Empty;

                    foreach (var item in files)
                    {
                        completeResponse += File.ReadAllText(item);
                    }

                    string[] responseArray = completeResponse.Split('\n');

                    int successCount = 0; int failureCount = 0; int routerOfflineCount = 0;
                    Get("0H", completeResponse, dtEnd, out successCount);
                    Get("b000H", completeResponse, dtEnd, out failureCount);
                    Get("c801H", completeResponse, dtEnd, out routerOfflineCount);
                    bool isErrored = false;
                    if (movingCount != successCount + failureCount)
                    {
                        isErrored = true;
                    }
                    if (ConfigurationManager.AppSettings["moveLevel"] == "SERIES")
                    {
                        File.AppendAllText(ConfigurationManager.AppSettings["logPath"], dtEnd.ToString() + '\t' + "Total series to be moved =" + movingCount + '\t' + "SuccessCount =" + successCount + '\t' + "FailureCount =" + failureCount + '\t' + " Is Errored = " + isErrored + Environment.NewLine);
                    }
                    else
                    {
                        File.AppendAllText(ConfigurationManager.AppSettings["logPath"], dtEnd.ToString() + '\t' + "Total study to be moved =" + movingCount + '\t' + "SuccessCount =" + successCount + '\t' + "FailureCount =" + failureCount + '\t' + " Is Errored = " + isErrored + Environment.NewLine);
                    }


                    dtEnd = dtEnd.AddDays(-1);
                }
                while (true)
                {

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static void RetryReponseReader()
        {
            string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["archivePath"], "*", SearchOption.AllDirectories);

            //String completeResponse = string.Empty;
            DateTime dtEnd = DateTime.Now;
            StringBuilder completeResponse = new StringBuilder(); int currentFile = 1;
            Console.WriteLine("Calculating success & failure");
            string[] codes = new string[] { "b000H" }; //, "b000H", "c801H", "fe00H" 
            int successCount = 0; int failureCount = 0;
            foreach (var code in codes)
            {
                completeResponse = new StringBuilder();
                long startSearch = 0;
                string status = code ;//b000H //0H
                int count = 0;
                List<string> lstStatus = new List<string>();
                foreach (var item in files)
                {
                    using (FileStream fs = File.Open(item, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (BufferedStream bs = new BufferedStream(fs))
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        String line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            completeResponse.AppendLine(line);
                            if (line.Contains("status=" + status))
                            {
                                count += 1;
                                long failurePosition = 0;
                                if (startSearch == 0)
                                {
                                    failurePosition = completeResponse.ToString().IndexOf("status=" + status, 0);
                                    startSearch = Convert.ToInt64(failurePosition + ("status=" + status).Length);
                                }
                                else
                                {
                                    failurePosition = completeResponse.ToString().IndexOf("status=" + status, (Int32)startSearch);
                                    startSearch = failurePosition + ("status=" + status).Length;
                                }

                                //SUID
                                long SUIDStartIndex = completeResponse.ToString().LastIndexOf("-m 0020000D=", (Int32)failurePosition) + "-m 0020000D=".Length;
                                long SUIDEndIndex = completeResponse.ToString().IndexOf(" ", (Int32)SUIDStartIndex);
                                string SUID = completeResponse.ToString().Substring((Int32)SUIDStartIndex, (Int32)SUIDEndIndex - (Int32)SUIDStartIndex);

                                string SEUID = string.Empty;
                                if (ConfigurationManager.AppSettings["moveLevel"] == "SERIES")
                                {
                                    //SEUID
                                    int SEUIDStartIndex = completeResponse.ToString().LastIndexOf("-m 0020000E=", (Int32)failurePosition) + "-m 0020000E=".Length;
                                    int SEUIDEndIndex = completeResponse.ToString().IndexOf(" ", SEUIDStartIndex);
                                    SEUID = completeResponse.ToString().Substring(SEUIDStartIndex, SEUIDEndIndex - SEUIDStartIndex);
                                }

                                //if (status == "b000H")
                                if (status != "0H")
                                {
                                    File.AppendAllText(ConfigurationManager.AppSettings["logPath"], dtEnd.ToString() + '\t' + SUID + '\t' + SEUID + Environment.NewLine);

                                }
                            }
                            if (line.Contains("status="))
                            {
                                string stat = line.Substring(line.IndexOf("status=") + "status=".Length);
                                if (!lstStatus.Contains(stat))
                                {
                                    lstStatus.Add(stat);
                                }
                            }
                        }
                    }
                    Console.WriteLine("Total file = " + files.Count() + " Completed file =" + currentFile);
                    ++currentFile;
                }
                if (code == "0H")
                {
                    Console.WriteLine("Success =" + count);
                    successCount = count;
                }
                if (code == "b000H")
                {
                    Console.WriteLine("Failure =" + count);
                    failureCount = count;
                }
                if (code == "c801H")
                {
                    Console.WriteLine("Unknown =" + count);
                    failureCount += count;
                }
                if (code == "fe00H")
                {
                    Console.WriteLine("Interupt =" + count);
                    failureCount += count;
                }

                foreach (var item in lstStatus)
                {
                    Console.WriteLine(item);
                }
                
            }
            
            string statusLogPath = ConfigurationManager.AppSettings["statusLogPath"];
            String[] SUIDList = File.ReadAllLines(statusLogPath);
            List<string> list = SUIDList.ToList().Where(z => !z.StartsWith("=") && !z.Contains("Total")).ToList();
            File.AppendAllText(ConfigurationManager.AppSettings["logPath"], dtEnd.ToString() + '\t' + "Total study to be moved =" + list.Count() + '\t' + "SuccessCount =" + successCount + '\t' + "FailureCount =" + failureCount + Environment.NewLine);
            while (true)
            {

            }
            
        }

        private static void CheckStatusCodes()
        {
            string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["archivePath"], "*", SearchOption.AllDirectories);
            List<string> lstCodes = new List<string>();
            int count = 0;
            foreach (var item in files)
            {
                count++;
                using (FileStream fs = File.Open(item, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (BufferedStream bs = new BufferedStream(fs))
                using (StreamReader sr = new StreamReader(bs))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("status="))
                        {
                            string statusCode = line.Substring(line.IndexOf("status="));
                            if (!lstCodes.Contains(statusCode))
                            {
                                lstCodes.Add(statusCode);
                                Console.WriteLine(statusCode);
                            }
                        }
                    }
                }
                Console.WriteLine(count);
            }
            while (true)
            {

            }
        }

        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        static void GetLogfiles(string logPath)
        {
            string[] files = Directory.GetFiles(logPath);
            foreach (var file in files)
            {
                FileInfo info = new FileInfo(file);
                File.Copy(logPath +"\\" + info.Name, ConfigurationManager.AppSettings["storeLogPath"] + Guid.NewGuid() +".txt");
            }

        }
    }
}