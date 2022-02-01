using System.IO;
using Tomlyn;
using Tomlyn.Model;


namespace Watcher {


    public static class ApplicationBuilder {

        public static Application BuildFromConfig(string configFilePath) {

            try {
                string fileContent = File.ReadAllText(configFilePath);
                var appInstance = Toml.ToModel<Application>(fileContent);
            }
            catch (TomlException e) {
                Console.WriteLine("TomlException: \n" + e.Message);
            }
            //TODO: Don't catch a general exception here
            catch (Exception e) {
                Console.WriteLine("Exception: " + e.Message);
            }


            return new Application();
        }
    }

}
