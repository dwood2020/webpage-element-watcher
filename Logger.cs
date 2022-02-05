using System;


namespace Watcher {

    /// <summary>
    /// Extremely simple Logger class. Can definitely be improved.
    /// </summary>
    public class Logger : ILogger {

        /// <summary>
        /// Logger verbosity level. Can be 1, 2, 3 (Error, Warning, Info)
        /// </summary>
        public int Verbosity { get; set; }

        /// <summary>
        /// Print XPath query results (for testing the XPath input) toggle
        /// </summary>
        public bool ShowXpathQueryResult { get; set; }


        public Logger() { }

        /// <summary>
        /// Logs an error following String.Format syntax.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="args">Parameters</param>
        public void Error(string message, params object[] args) {
            Output(String.Format(message, args), ConsoleColor.Red);
        }

        /// <summary>
        /// Logs a warning following String.Format syntax.
        /// </summary>
        /// <param name="message">Warning message</param>
        /// <param name="args">Parameters</param>
        public void Warning(string message, params object[] args) { 
            if (Verbosity > 1) {
                Output(String.Format(message, args), ConsoleColor.Yellow);
            }
            
        }

        /// <summary>
        /// Logs an info following String.Format syntax.
        /// </summary>
        /// <param name="message">Info message</param>
        /// <param name="args">Parameters</param>
        public void Info(string message, params object[] args) {
            if (Verbosity > 2) {
                Output(String.Format(message, args), ConsoleColor.White);
            }
        }

        /// <summary>
        /// Logs an XPath query result. 
        /// Meant to be called from a job instance.
        /// </summary>
        /// <param name="jobname">Job name</param>
        /// <param name="result">XPath query result</param>
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