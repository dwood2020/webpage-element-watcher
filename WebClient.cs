using System.Net;

namespace Watcher {

    // thin, KISS wrapper around .net httpclient as singleton (not nice but easy)
    //TODO: Comment + exchange Singleton patten to be used with Framework Dependency Injection
    //there probably is no real reason to make the Client singleton here 
    public class WebClient {

        private static WebClient? instance;

        private HttpClient client;

        public WebClient() {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Console Program");
        }

        /// <summary>
        /// Gets the HTML content from a provided URL. Aync method.
        /// </summary>
        /// <param name="url">URL to get content from</param>
        /// <returns>Task object</returns>
        public async Task<string> GetHtml(string url) {
            var content = await client.GetStringAsync(url);
            return content;
        }

        /// <summary>
        /// Gets the Singleton instance
        /// </summary>
        /// <returns>WebClient instance</returns>
        public static WebClient GetInstance() {
            if (instance == null) {
                instance = new WebClient();
            }
            return instance;
        }
    }

}