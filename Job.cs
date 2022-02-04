using HtmlAgilityPack;


namespace Watcher {

    /// <summary>
    /// This class represents a watcher job
    /// </summary>
    public class Job {
        
        /// <summary>
        /// The URL to the webpage
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// The XPath Query to find the HTML element within the doc
        /// </summary>
        public string? Xpath { get; set; }

        /// <summary>
        /// The job result: Inner HTML of the XPath Query result
        /// </summary>
        public string? Result { get; private set; }

        private readonly HtmlDocument htmlDoc;


        public Job() {
            htmlDoc = new HtmlDocument();   //TODO: Check if it is ok to use 1 instance for each job Run
         }


        /// <summary>
        /// Run the Job (Async method)
        /// </summary>
        /// <returns>Async task</returns>
        public async Task Run() {

            // this can be done in async method, see
            // here: https://stackoverflow.com/questions/25055749/terminate-or-exit-c-sharp-async-method-with-return
            if (Url == null || Xpath == null || Xpath.Length == 0) {
                return;
            }

            string html = await WebClient.GetInstance().GetHtml(Url);

            htmlDoc.LoadHtml(html);
            // simply utilise XPath Sysntax here, see
            // https://www.w3schools.com/xml/xpath_syntax.asp
            var nodes = htmlDoc.DocumentNode.SelectNodes(Xpath);

            if (nodes == null) {
                Logger.GetInstance().Write("Job: No nodes found. Returning.");
                return;
            }
            Logger.GetInstance().Write(String.Format("Job: Found {0} matching nodes.", nodes.Count));

            foreach (var node in nodes.ToList()) {
                Logger.GetInstance().Write(String.Format("Matching HTML Node Content: {0}", node.InnerHtml));
            }
        }
        
    }
}