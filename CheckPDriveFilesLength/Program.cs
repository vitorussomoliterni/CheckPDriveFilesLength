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
            var selection = new ConsoleKeyInfo();
            var appSettings = ConfigurationManager.AppSettings;

            while (true)
            {
                GetMainMenu();
                selection = Console.ReadKey();

                if (selection.KeyChar.Equals('1')) // Checks specific folder
                {
                    Console.WriteLine("\n");
                    GetSpecificProjectFileList();
                }
                else if (selection.KeyChar.Equals('2')) // Checks whole P Drive
                {
                    Console.WriteLine("\n");
                    GetLongFileNamesList(appSettings["searchPath"], appSettings["logPath"] + "_" + DateTime.Now.ToShortDateString() + ".csv");
                }
                else if (selection.KeyChar.Equals('3')) // Exits application
                {
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("\nNo matches for your input, try again\n");
                }
            }
        }

        private static void GetSpecificProjectFileList()
        {
            var selection = "";
            var appSettings = ConfigurationManager.AppSettings;
            while (selection.Equals(""))
            {
                Console.Write("Please insert project number and press enter (type 'back' to go back): ");
                selection = Console.ReadLine().Trim();
                int i;
                bool selectionIsInt = int.TryParse(selection, out i); // Checks if user input is parsable

                if (selection.Length == 5 && selectionIsInt) // Checks if user input consists in 5 numbers
                {
                    var projectPath = GetProjectPath(selection); // Gets project path
                    if (projectPath == null)
                    {
                        selection = "";
                        Console.WriteLine("No project found with that number");
                    }
                    else
                    {
                        GetLongFileNamesList(projectPath, appSettings["logPath"] + "_" + selection + ".csv");
                    }
                }
                else if (selection.Trim().ToLower().Equals("back"))
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("\nInvalid project number\n");
                    selection = "";
                }
            }
        }

        private static string GetProjectPath(string projectNumber)
        {
            var appSettings = ConfigurationManager.AppSettings;
            var searchPattern = "*" + projectNumber + "_*";
            var searchPath = appSettings["searchPath"] + "20" + projectNumber.Substring(0, 2);

            try
            {
                var project = from file in Directory.EnumerateDirectories(searchPath, searchPattern, SearchOption.TopDirectoryOnly)
                              select new
                              {
                                  Name = file
                              };

                foreach (var item in project) // Returns the first path found for that project
                {
                    return item.Name;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return null;
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

        private static void GetLongFileNamesList(string searchPath, string logPath)
        {
            var appStart = DateTime.Now;
            var appSettings = ConfigurationManager.AppSettings;
            var searchCriteria = "*.*";
            var charactersLimit = 245;

            var logText = DateTime.Now.ToShortDateString() + "\n";

            Console.WriteLine("Scanning " + searchPath + " for files with names longer than " + charactersLimit + " characters...");

            try
            {
                var files = from file in Directory.EnumerateFiles(searchPath, searchCriteria, SearchOption.AllDirectories) // Looks for files with names bigger than 245 characters
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
                Log(logText, logPath);
            }

            var appEnd = DateTime.Now;
            var runningTime = appEnd - appStart;

            Console.WriteLine("Scan finished");
            Console.WriteLine("Running time: {0:c}", runningTime);
            Console.WriteLine("Press any key to go back to main menu");
            Console.ReadKey();
        }
    }
}
