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
                // app = Toml.ToModel<Application>(fileContent);
                // app.Init();

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

            // build app from config here
            // note this is no C++ - we can create objects without having to worry about ownership

            // this is really dirty now of course
            app = new Application();
            app.Logger = new Logger();
            app.Database = new Database();

            if (appConfig.Jobs != null) {
                foreach (JobConfig jc in appConfig.Jobs) {
                    app.Jobs.Add(new Job() {
                        Name = jc.Name,
                        Url = jc.Url,
                        Xpath = jc.XPath,
                        ResultType = jc.ResultType
                    });
                }    
            }
            
            // TODO: CLean this mess up.
            // Add methods for each class setup which take in config class
            // call them one by one 
            // think about dependencies + unit testing => what do i need to hide behind interfaces?

            return app;
        }


        private static ILogger BuildLogger(LoggerConfig config) {
            //NOTE: The factory actually knows the concrete type and not just the Interface
            Logger logger = new Logger() {                
                ShowXpathQueryResult = config.ShowXpathQueryResult,
            };

            if (Enum.IsDefined(typeof(LoggerVerbosity), config.Verbosity)) { 
                logger.Verbosity = (LoggerVerbosity)config.Verbosity;
            }

            return logger;
        }

        private static Database BuildDatabase(DatabaseConfig config) {

            Database db = new() {
                Path = config.Path,
            };

            return db;
        }
    }

}
