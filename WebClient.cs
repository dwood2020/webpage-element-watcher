using System.Net;

namespace Watcher {

    //TODO: Comment interface
    public interface IWebClient {

        public Task<string> GetHtml(string url);
    }


    /// <summary>
    /// This class serves as a thin wrapper around the .NET HttpClient
    /// </summary>
    public class WebClient : IWebClient {

        private ILogger logger;    

        private HttpClient client;

        public WebClient(ILogger logger) {
            this.logger = logger;
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Console Program");
        }

        /// <summary>
        /// Gets the HTML content from a provided URL. Async method.
        /// </summary>
        /// <param name="url">URL to get content from</param>
        /// <returns>Task object</returns>
        public async Task<string> GetHtml(string url) {
            string content;
            try {
                content = await client.GetStringAsync(url);
            }
            catch(AggregateException e) {
                logger.Error("WebClient: Exception: \n" + e.Message);
                content = "";
            }
            return content;
        }
    }

}