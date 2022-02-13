namespace Watcher.Config {
    
    class ApplicationConfig {
        public long IntervalSeconds { get; set; } = 0;
        public LoggerConfig? Logger { get; set; }
        public UserConfig? User { get; set; }       
        public DatabaseConfig? Database { get; set; }
        public List<JobConfig> Jobs { get; set; }

        public ApplicationConfig() {
            Jobs = new List<JobConfig>();
        }
    }


    public class UserConfig {
        public string Name { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;

        public UserConfig() { }
    }


    public class DatabaseConfig {
        public string Path { get; set; } = string.Empty;

        public DatabaseConfig() { }
    }

    public class LoggerConfig {
        public long Verbosity { get; set; } = 0;
        public bool ShowXpathQueryResult { get; set; } = false;

        public LoggerConfig() { }
    }


    public class JobConfig {
        // NOTE: These simple strings are used instead of a proper enum to make TOML parsing easy and automatic
        public static readonly string ResultTypeNumber = "number";
        public static readonly string ResultTypeString = "string";

        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string LocalPath { get; set; } = string.Empty;
        public string XPath { get; set; } = string.Empty;
        public string ResultType { get; set; } = string.Empty;

        public JobConfig() { }
    }


}