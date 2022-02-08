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
            
            // TODO: CLean this mess up.
            // Add methods for each class setup which take in config class
            // call them one by one 
            // think about dependencies + unit testing => what do i need to hide behind interfaces?
            
            ILogger logger = BuildLogger(appConfig.Logger);

            IDatabase database = BuildDatabase(logger, appConfig.Database);


            //IDEA: 
            // Build the logger first. Then, inject the logger dependency via ctor in all other components
            // Do a graceful default fallback init if any config params aren't set
            // Hold defaults in the classes themselves or in the Config classes ?


            return app;
        }


        private static ILogger BuildLogger(LoggerConfig? config) {
            
            //TODO: A design decision: Fail with an Exception or handle missing config gracefully
            // via default parameters?

            if (config == null) {
                //throw new Exception("Logger config invalid");
                config = new LoggerConfig();    // use default values here
            }

            //NOTE: The factory actually knows the concrete type and not just the Interface
            Logger logger = new Logger() {                
                ShowXpathQueryResult = config.ShowXpathQueryResult,
            };

            if (Enum.IsDefined(typeof(LoggerVerbosity), config.Verbosity)) { 
                logger.Verbosity = (LoggerVerbosity)config.Verbosity;
            }

            return logger;
        }

        private static Database BuildDatabase(ILogger logger, DatabaseConfig? config) {
            
            if (config == null || config.Path.Length == 0) {
                throw new Exception("Database config invalid");
            }

            return new Database(logger, config.Path);
        }
    }

}
