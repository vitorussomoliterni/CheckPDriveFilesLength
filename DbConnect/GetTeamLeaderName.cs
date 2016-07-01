using System;
using MySql.Data.MySqlClient;
using System.Data;

namespace DbConnect
{
    public static class GetTeamLeaderName
    {
        public static string GetName(string projectNo)
        {
            var details = new Connector();
            string connStr = details.ConnectionString;

            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr))
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        Console.WriteLine("Connecting to MySQL...");
                        con.Open();
                    }

                    var getLeaderId = "SELECT team_leader_id FROM " + details.ProjectsTable + " WHERE number = " + projectNo + ";";
                    MySqlCommand getLeaderIdCmd = new MySqlCommand(getLeaderId, con);
                    var leaderId = getLeaderIdCmd.ExecuteScalar().ToString();
                    Console.WriteLine(leaderId);

                    var getUserId = "SELECT user_id FROM " + details.UserProfilesTable + " WHERE id = " + leaderId + ";";
                    MySqlCommand getUserIdCmd = new MySqlCommand(getUserId, con);
                    var userId = getUserIdCmd.ExecuteScalar().ToString();
                    Console.WriteLine(userId);

                    var getLeaderLastName = "SELECT last_name FROM " + details.UsersTable + " WHERE id = " + userId + ";";
                    MySqlCommand getLeaderLastNameCmd = new MySqlCommand(getLeaderLastName, con);
                    var leaderLastName = getLeaderLastNameCmd.ExecuteScalar().ToString();
                    Console.WriteLine(leaderLastName);

                    return leaderLastName;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
    }
}
