using System.IO;
using Tomlyn;
using Watcher.Config;


namespace Watcher {


    /// <summary>
    /// This class acts as factory for the application instance. 
    /// It initialises the instance with the data from the Config file.
    /// It is implemented as static class as it has no internal state.
    /// </summary>
    public static class ApplicationBuilder {

        /// <summary>
        /// Creates an application instance from configuration file.
        /// Should only be called once.        
        /// </summary>
        /// <param name="configFilePath">Path to config file</param>
        /// <returns>Application instance</returns>
        public static Application BuildFromConfig(string configFilePath) {

            Application app;
            ApplicationConfig appConfig;
            try {
                string fileContent = File.ReadAllText(configFilePath);
                appConfig = Toml.ToModel<ApplicationConfig>(fileContent);
            }
            catch (TomlException e) {
                Console.WriteLine("TomlException: \n" + e.Message);
                throw e;
            }
            //TODO: Don't catch a general exception here
            catch (Exception e) {
                Console.WriteLine("Exception: " + e.Message);
                throw;
            }

            // check if any nulls / non-defined settings here
            if (!CheckConfig(appConfig, out string message)) {
                throw new InvalidDataException(message);
            }


            //NOTE: We disable this warning here because null-checks are performed via CheckCofig()
#pragma warning disable CS8604 // Possible null reference
            
            // Build the logger first
            ILogger logger = BuildLogger(appConfig.Logger);
            IWebClient webClient = BuildWebClient();
            IDatabase database = BuildDatabase(logger, appConfig.Database);
            IUser user = BuildUser(appConfig.User);
            List<IJob> jobs = BuildJobs(logger, webClient, appConfig.Jobs);

#pragma warning restore CS8604            

            app = new(logger, database, user, jobs);

            return app;
        }

        private static bool CheckConfig(ApplicationConfig appConfig, out string message) { 
            // NOTE: This is probably not so nice. Refactor / change concept?
            if (appConfig.Logger == null) {
                message = "Logger config not present";
                return false;
            }
            if (appConfig.User == null) {
                message = "User config not present";
                return false;
            }
            if (appConfig.Database == null) {
                message = "Database config not present";
                return false;
            }
            if (appConfig.Jobs == null) {
                message = "No job configs present";
                return false;
            }
            message = string.Empty;
            return true;
        }

        private static ILogger BuildLogger(LoggerConfig config) {

            int verbosity = 3;
            if (config.Verbosity >= 1 && config.Verbosity <= 3) { 
                verbosity = (int)config.Verbosity;
            }

            //NOTE: The factory actually knows the concrete type and not just the Interface
            Logger logger = new() {                
                Verbosity = verbosity,
                ShowXpathQueryResult = config.ShowXpathQueryResult
            };

            return logger;
        }

        private static IDatabase BuildDatabase(ILogger logger, DatabaseConfig config) {
            // this is a simple task for now
            return new Database(logger, config.Path);
        }

        private static IUser BuildUser(UserConfig user) {

            User instance = new User() {
                Name = user.Name,
                Mail = user.Mail,
            };

            return (IUser)instance;
        }

        private static IWebClient BuildWebClient() {
            return new WebClient();
        }

        private static List<IJob> BuildJobs(ILogger logger, IWebClient webClient, List<JobConfig> jobConfigs) {
            List<IJob> jobs = new();

            foreach (JobConfig jc in jobConfigs) {
                Job j = new(logger, webClient) {
                    Name = jc.Name,
                    Url = jc.Url,
                    Xpath = jc.XPath,
                };

                if (jc.ResultType == JobConfig.ResultTypeNumber) {
                    j.TreatAsNumber = true;
                }

                jobs.Add(j);
            }
            return jobs;
        }

    }

}
