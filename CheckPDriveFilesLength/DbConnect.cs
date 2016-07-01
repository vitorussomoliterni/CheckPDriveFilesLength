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
            string connStr = appSettings["connectionString"];
            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                var getLeaderId = "SELECT team_leader_id FROM " + appSettings["projectsTable"] + " WHERE number = " + projectNo + ";";
                SqlCommand getLeaderIdCmd = new SqlCommand(getLeaderId, conn);
                var leaderId = getLeaderIdCmd.ExecuteScalar().ToString();

                var getUserId = "SELECT user_id FROM " + appSettings["userProfilesTable"] + " WHERE id = " + leaderId + ";";
                SqlCommand getUserIdCmd = new SqlCommand(getUserId, conn);
                var userId = getUserIdCmd.ExecuteScalar().ToString();

                var getLeaderLastName = "SELECT last_name FROM " + appSettings["usersTable"] + " WHERE id = " + userId + ";";
                SqlCommand getLeaderLastNameCmd = new SqlCommand(getLeaderLastName, conn);
                var leaderLastName = getLeaderLastNameCmd.ExecuteScalar().ToString();

                conn.Close();

                return leaderLastName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            conn.Close();
            return null;
        }
    }

    
}
