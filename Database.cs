using System;
using Microsoft.Data.Sqlite;


namespace Watcher {


    public class Database {

        public string Path { get; set; }

        private SqliteConnection connection;

        public Database() {
            Path = String.Empty;
            connection = new SqliteConnection();
        }

    }
}
