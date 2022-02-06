using System;
using Microsoft.Data.Sqlite;
using System.Text;


namespace Watcher {


    public class Database {

        public string Path { get; set; }

        private SqliteConnection connection;

        private ILogger? logger;

        public Database() {
            Path = String.Empty;
            connection = new SqliteConnection();
        }

        public void SetLogger(ILogger logger) {
            this.logger = logger;
        }

        public void InsertJobResult<T>(string jobName, JobResult<T> result) {
            connection.Open();

            // table
            var cmdTable = connection.CreateCommand();
            cmdTable.CommandText = @"CREATE TABLE IF NOT EXISTS " + ScrubSqlParameter(jobName) + "(id INTEGER PRIMARY KEY AUTOINCREMENT, timestamp STRING, content STRING);";
            cmdTable.ExecuteNonQuery();

            // values
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO " + ScrubSqlParameter(jobName) + "(timestamp, content) VALUES ($timestamp, $content);";
            cmd.Parameters.AddWithValue("timestamp", result.Timestamp);
            if (result.Content != null) {
                cmd.Parameters.AddWithValue("content", result.Content.ToString());
            }
            else {
                logger?.Error("Database: JobResult Content was null. Could not insert record into table {0}", jobName);
            }

            connection.Close();
        }

        private string ScrubSqlParameter(string param) {
            StringBuilder sb = new();            

            foreach (char c in param) {
                if (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c >= '0' && c <= '9') {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

    }
}
