using System;
using System.Data.SqlClient;
using Microsoft.Deployment.WindowsInstaller;

namespace FalconOrchestrator.Installer.CustomActions
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CheckDatabaseExistence(Session session)
        {
            session.Log("Begin CheckDatabaseExistence");

            string connectionString = @"Data Source=" + session["DB_SERVER"] + ";Connect Timeout=60;Initial Catalog=" + session["DB_DATABASE"] + @";Persist Security Info=True;User ID=" + session["DB_USER"] + ";Password=" + session["DB_PASSWORD"];
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                connection.Close();
                session["SQLDATABASEEXISTS"] = "true";
            }
            catch (Exception ex)
            {
                session.Log("Database connection not possible. Database treated as not existing. Exception: " + ex.Message);
                session["SQLDATABASEEXISTS"] = "false";
            }

            session.Log("Database exists: " + session["SQLDATABASEEXISTS"]);

            return ActionResult.Success;
        }
    }
}
