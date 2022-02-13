using System;
using Microsoft.Data.Sqlite;
using System.Text;


namespace Watcher {

    public interface IDatabase {
        public string Path { get; set; }

        public void InsertJobResult(string jobName, JobResult result);
    }


    public class Database : IDatabase {

        public string Path { get; set; }

        private SqliteConnection connection;

        private ILogger logger;

        public Database(ILogger logger, string filepath) {
            Path = filepath;
            this.logger = logger;
            connection = new SqliteConnection("Data Source=" + Path);
        }

        public void InsertJobResult(string jobName, JobResult result) {

            if (result == null) {
                logger.Error("Database: JobResult was null for Job \"{0}\".", jobName);
                return;
            }

            string jobnameScrubbed = ScrubSqlParameter(jobName);
            if (jobnameScrubbed.Length == 0) {
                logger.Error("Database: Job name \"{0}\" was empty after scrubbing. Cannot insert result into database", jobName);
                return;
            }

            connection.Open();

            // table
            var cmdTable = connection.CreateCommand();
            cmdTable.CommandText = @"CREATE TABLE IF NOT EXISTS " + jobnameScrubbed + "(id INTEGER PRIMARY KEY AUTOINCREMENT, timestamp STRING, content STRING);";
            cmdTable.ExecuteNonQuery();

            // values
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO " + jobnameScrubbed + "(timestamp, content) VALUES ($timestamp, $content);";
            cmd.Parameters.AddWithValue("timestamp", result.Timestamp);
            if (result.Content != null) {
                cmd.Parameters.AddWithValue("content", result.Content.ToString());
                cmd.ExecuteNonQuery();
            }
            else {
                logger.Error("Database: JobResult Content was null. Could not insert record into table {0}", jobName);
            }

            connection.Close();
        }

        public List<JobResult> GetLastJobResults(string jobName, int limit) {            

            List<JobResult> results = new();

            string jobnameScrubbed = ScrubSqlParameter(jobName);
            if (jobnameScrubbed.Length == 0) {
                logger.Error("Database: Job name \"{0}\" was empty after scrubbing. Cannot insert result into database", jobName);
                return results;
            }

            connection.Open();

            // check if table exists - is this even neccessary??
            //var cmdTableExist = connection.CreateCommand();
            //cmdTableExist.CommandText = @"SELECT name FROM sqlite_master WHERE type='table' AND name='$(tablename)';";
            //cmdTableExist.Parameters.AddWithValue("tablename", jobnameScrubbed);

            //using (var reader = cmdTableExist.ExecuteReader()) {
            //    if (!reader.Read()) {
            //        logger.Error("Database: table for (scrubbed) Job name \"{0}\" does not exist.", jobnameScrubbed);
            //        return results;
            //    }
            //}

            // get results
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM " + jobnameScrubbed + " ORDER BY id DESC LIMIT $limit;";
            cmd.Parameters.AddWithValue("limit", limit);

            using (var reader = cmd.ExecuteReader()) {
                while (reader.Read()) {
                    string timestamp = reader.GetString(1);
                    string content = reader.GetString(2);

                    results.Add(new JobResult(timestamp, content));
                }
            }

            connection.Close();

            return results;
        }

        private string ScrubSqlParameter(string param) {
            StringBuilder sb = new();            

            foreach (char c in param) {
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')) {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

    }
}
