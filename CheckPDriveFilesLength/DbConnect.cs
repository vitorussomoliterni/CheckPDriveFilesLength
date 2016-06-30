using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckPDriveFilesLength
{
    public static class DbConnect
    {
        public static string GetTeamLeaderName(string projectNo)
        {
            var appSettings = ConfigurationManager.AppSettings;

            var getLeaderId = "SELECT team_leader_id FROM " + appSettings["projectsTable"] + " WHERE number = " + projectNo + ";";
            var leaderId = Connect(getLeaderId);

            var getUserId = "SELECT user_id FROM " + appSettings["userProfilesTable"] + " WHERE id = " + leaderId + ";";
            var userId = Connect(getUserId);

            var getLeaderLastName = "SELECT last_name FROM " + appSettings["usersTable"] + " WHERE id = " + userId + ";";
            var leaderLastName = Connect(getLeaderLastName);

            return leaderLastName;
        }

        public static string Connect(string query)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string connStr = appSettings["connectionString"];
            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();
                // Perform database operations

                SqlCommand cmd = new SqlCommand(query, conn);

                var result = cmd.ExecuteScalar().ToString();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            conn.Close();
            Console.WriteLine("Done.");
            return null;
        }
    }

    
}
