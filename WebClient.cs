using System.Net;

namespace Watcher {

    // thin, KISS wrapper around .net httpclient as singleton (not nice but easy)
    public class WebClient {

        private static WebClient? instance;

        private HttpClient client;

        public WebClient() {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Console Program");
        }

        public async Task<string> GetHtml(string url) {
            var content = await client.GetStringAsync(url);
            return content;
        }

        public static WebClient GetInstance() {
            if (instance == null) {
                instance = new WebClient();
            }
            return instance;
        }
    }

}