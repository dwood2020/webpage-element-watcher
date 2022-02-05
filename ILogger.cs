
namespace Watcher {

    /// <summary>
    /// Simple logger interface.
    /// It is used to pass the logger instance as dependency
    /// and hide the actual logger implementation from its callers.
    /// </summary>
    public interface ILogger {

        /// <summary>
        /// Logs an error following String.Format syntax.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="args">Parameters</param>
        public void Error(string message, params object[] args);

        /// <summary>
        /// Logs a warning following String.Format syntax.
        /// </summary>
        /// <param name="message">Warning message</param>
        /// <param name="args">Parameters</param>
        public void Warning(string message, params object[] args);

        /// <summary>
        /// Logs an info following String.Format syntax.
        /// </summary>
        /// <param name="message">Info message</param>
        /// <param name="args">Parameters</param>
        public void Info(string message, params object[] args);

        /// <summary>
        /// Logs an XPath query result. 
        /// Meant to be called from a job instance.
        /// </summary>
        /// <param name="jobname">Job name</param>
        /// <param name="result">XPath query result</param>
        public void XpathQueryResult(string jobname, string result);
    }
}
