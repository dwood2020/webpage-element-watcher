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

            IDatabase database = BuildDatabase(logger, appConfig.Database);
            IUser user = BuildUser(appConfig.User);

#pragma warning restore CS8604

            List<Job> jobs = new List<Job>();

            foreach (JobConfig jc in appConfig.Jobs) {
                if (jc.ResultType == JobConfig.ResultTypeNumber) {
                    jobs.Add(new NumberJob(logger));
                }
                else {
                    jobs.Add(new StringJob(logger));
                }
            }

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

    }

}
