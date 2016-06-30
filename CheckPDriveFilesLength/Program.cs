using System;
using System.IO;
using System.Linq;
using System.Configuration;

namespace CheckPDriveFilesLength
{
    class Program
    {
        static void Main(string[] args)
        {
            var appStart = DateTime.Now;
            var appSettings = ConfigurationManager.AppSettings;

            var searchPath = appSettings["searchPath"];
            var searchCriteria = "*.*";
            var charactersLimit = 245;

            var logText = DateTime.Now.ToShortDateString() + "\n";

            var logPath = appSettings["logPath"];

            Console.WriteLine("Scanning " + searchPath + " for files with names longer than " + charactersLimit + " characters...");

            try
            {
                var files = from file in Directory.EnumerateFiles(searchPath, searchCriteria, SearchOption.AllDirectories)
                            where file.Length > charactersLimit
                            select new
                            {
                                File = file
                            };

                foreach (var f in files)
                {
                    logText += f.File + ";";
                    logText += f.File.Length + "\n";
                }
            }
            catch (Exception e)
            {
                logText += "Application crashed\n";
                logText += "Error message: " + e.Message.ToString();
            }

            finally
            {
                using (StreamWriter w = File.AppendText(logPath))
                {
                    Log(logText, w);
                }
            }

            var appEnd = DateTime.Now;
            var runningTime = appEnd - appStart;

            Console.WriteLine("Scan finished");
            Console.WriteLine("Running time: {0:c}", runningTime);
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.WriteLine(logMessage);
            w.WriteLine("-------------------------------");
        }
    }
}
