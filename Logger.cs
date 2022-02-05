using System;


namespace Watcher {

    /// <summary>
    /// Extremely simple Logger class. Can definitely be improved.
    /// </summary>
    public class Logger : ILogger {

        public int Verbosity { get; set; }

        public bool ShowXpathQueryResult { get; set; }


        public Logger() { }

        public void Error(string message, params object[] args) {
            Output(String.Format(message, args), ConsoleColor.Red);
        }

        public void Warning(string message, params object[] args) { 
            if (Verbosity > 1) {
                Output(String.Format(message, args), ConsoleColor.Yellow);
            }
            
        }

        public void Info(string message, params object[] args) {
            if (Verbosity > 2) {
                Output(String.Format(message, args), ConsoleColor.White);
            }
        }

        public void XpathQueryResult(string jobname, string result) {
            if (ShowXpathQueryResult || Verbosity > 1) {
                Output(String.Format("Job \"{0}\" XPath Query Result (Inner HTML): {1}", jobname, result), ConsoleColor.Cyan);
            }
        }

        private void Output(string message, ConsoleColor color) {
            string timestamp = DateTime.Now.ToString("T");
            string separator = " | ";

            ConsoleColor previousColor = Console.ForegroundColor;
            if (color != ConsoleColor.White) {
                Console.ForegroundColor = color;
            }
            Console.WriteLine(timestamp + separator + message);
            Console.ForegroundColor = previousColor;
            // add file access here?
        }        

    }
}