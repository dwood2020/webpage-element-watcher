namespace Watcher {

    public class Logger {

        public bool Verbose { get; set; }

        private static Logger? instance;

        private Logger() {
            Verbose = false;
        }

        //TODO: Enable formatting here
        public void Write(string msg) {
            if (!Verbose) {
                return;
            }
            Console.WriteLine(msg);
        }

        public static Logger GetInstance() {
            if (instance == null) {
                instance = new Logger();
            }
            return instance;
        }

    }
}