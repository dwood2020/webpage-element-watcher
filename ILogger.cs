
namespace Watcher {

    /// <summary>
    /// Simple logger interface.
    /// It is used to pass the logger instance as dependency
    /// and hide the actual logger implementation from its callers.
    /// </summary>
    public interface ILogger {

        public void Error(string message, params object[] args);

        public void Info(string message, params object[] args);

        public void XpathQueryResult(string jobname, string result);
    }
}
