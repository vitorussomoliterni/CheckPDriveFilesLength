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
                        con.Open();
                    }

                    var getLeaderId = "SELECT team_leader_id FROM " + details.ProjectsTable + " WHERE number = " + projectNo + ";";
                    MySqlCommand getLeaderIdCmd = new MySqlCommand(getLeaderId, con);
                    var leaderId = getLeaderIdCmd.ExecuteScalar().ToString();

                    if (leaderId.Equals(""))
                    {
                        return null;
                    }

                    var getUserId = "SELECT user_id FROM " + details.UserProfilesTable + " WHERE id = " + leaderId + ";";
                    MySqlCommand getUserIdCmd = new MySqlCommand(getUserId, con);
                    var userId = getUserIdCmd.ExecuteScalar().ToString();

                    if (userId.Equals(""))
                    {
                        return null;
                    }

                    var getLeaderLastName = "SELECT last_name FROM " + details.UsersTable + " WHERE id = " + userId + ";";
                    MySqlCommand getLeaderLastNameCmd = new MySqlCommand(getLeaderLastName, con);
                    var leaderLastName = getLeaderLastNameCmd.ExecuteScalar().ToString();

                    if (leaderLastName.Equals(""))
                    {
                        return null;
                    }

                    var getLeaderPhoneNumber = "SELECT phone_number FROM " + details.UserProfilesTable + " WHERE id = " + userId + ";";
                    MySqlCommand getLeaderPhoneNumberCmd = new MySqlCommand(getLeaderPhoneNumber, con);
                    var leaderPhoneNumber = getLeaderPhoneNumberCmd.ExecuteScalar().ToString();

                    if (leaderLastName.Equals(""))
                    {
                        leaderPhoneNumber = null;
                    }

                    return leaderLastName + ";" + leaderPhoneNumber;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MySQL error: {0}", ex.Message);
            }
            return null;
        }
    }
}
