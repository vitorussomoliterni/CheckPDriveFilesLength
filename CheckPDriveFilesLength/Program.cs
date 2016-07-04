using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Data;
using DbConnect;

namespace CheckPDriveFilesLength
{
    class Program
    {
        public static string lastProjectChecked = "";
        public static string lastProjectLeader = "";
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
                var files = from file in Directory.EnumerateFiles(searchPath, searchCriteria, SearchOption.AllDirectories) // Looks for files with names bigger than 245
                            where file.Length > charactersLimit
                            select new
                            {
                                File = file
                            };

                foreach (var f in files) // Iterates through the results
                {
                    logText += f.File + ";";
                    logText += f.File.Length;
                    var projectNo = GetProjectNumber(f.File);
                    if (projectNo == lastProjectChecked) // Checks if the project leader has already been retrieved
                    {
                        logText += ";" + lastProjectLeader + "\n";
                    }
                    else
                    {
                        var teamLeaderLastName = GetTeamLeaderName.GetName(projectNo);
                        if (teamLeaderLastName == null)
                        {
                            logText += ";None\n";
                            lastProjectLeader = "None";
                        }
                        else
                        {
                            logText += ";" + teamLeaderLastName + "\n";
                            lastProjectLeader = teamLeaderLastName;
                        }
                        lastProjectChecked = projectNo;
                    }
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

        public static string GetProjectNumber(string fileName)
        {
            var projectNo = fileName.Substring(8, 5);
            return projectNo;
        }
    }
}
