using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Data;
using DbConnect;

namespace CheckPDriveFilesLength
{
    public class Program
    {
        private static string lastProjectChecked = "";
        private static string lastProjectLeader = "";
        static void Main(string[] args)
        {
            var selection = "";

            while(selection == "")
            {
                GetMainMenu();
                selection = Console.ReadLine();

                if (selection.Trim().Equals("1")) // Check specific folder
                {
                    GetSpecificProjectFileList();
                }
                else if (selection.Trim().Equals("2")) // Check whole P Drive
                {
                    GetLongFileNamesList();
                }
                else if (selection.Trim().Equals("3")) { } // Exit application
                else
                {
                    selection = "";
                    Console.WriteLine("\nNo matches for your input, try again\n");
                }
            }
        }

        private static void GetSpecificProjectFileList()
        {
            var selection = "";
            while (selection == "")
            {
                Console.Write("Please insert project number: ");
                selection = Console.ReadLine().Trim();


            }
        }

        private static void GetMainMenu()
        {
            Console.WriteLine("Check files that exceed Windows file name length limit\n");
            Console.WriteLine("1. Check a specific project");
            Console.WriteLine("2. Scan whole P Drive");
            Console.WriteLine("3. Exit\n");
            Console.Write("Make your selection: ");
        }

        private static void Log(string message, string path)
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

        private static string GetProjectNumber(string fileName)
        {
            var projectNo = fileName.Substring(8, 5);
            return projectNo;
        }

        private static void GetLongFileNamesList()
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
    }
}
