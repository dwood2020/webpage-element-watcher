using System.IO;
using Tomlyn;
using Tomlyn.Model;


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
            try {
                string fileContent = File.ReadAllText(configFilePath);
                app = Toml.ToModel<Application>(fileContent);
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

            return app;
        }
    }

}
