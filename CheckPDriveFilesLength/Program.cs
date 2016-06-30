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
                logText += "Error message: " + e.ToString();
                Console.WriteLine("Error: " + e.ToString());
            }

            finally
            {
                Log(logText, appSettings["logPath"]);
            }

            var appEnd = DateTime.Now;
            var runningTime = appEnd - appStart;

            Console.WriteLine("Scan finished");
            Console.WriteLine("Running time: {0:c}", runningTime);
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        public static void Log(string message, string path)
        {
            try
            {
                using (TextWriter w = File.CreateText(path))
                {
                    w.WriteLine(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not log the file: " + e.ToString());
            }
        }
    }
}
