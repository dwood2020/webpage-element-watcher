namespace Watcher.Config {
    
    class ApplicationConfig {
        public long IntervalSeconds { get; set; }
        public UserConfig? User { get; set; }
        public LoggerConfig? Logger { get; set; }        
        public DatabaseConfig? Database { get; set; }
        public List<JobConfig>? Jobs { get; set; }

        public ApplicationConfig() { }
    }


    public class UserConfig {
        public string? Name { get; set; }
        public string? Mail { get; set; }

        public UserConfig() { }
    }


    public class DatabaseConfig {
        public string Path { get; set; } = String.Empty;

        public DatabaseConfig() { }
    }

    public class LoggerConfig {
        public long Verbosity { get; set; }
        public bool ShowXpathQueryResult { get; set; }

        public LoggerConfig() { }
    }


    public class JobConfig {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? XPath { get; set; }
        public string? ResultType { get; set; }

        public JobConfig() { }
    }


}