using HtmlAgilityPack;


namespace Watcher {

    /// <summary>
    /// This class represents a watcher job
    /// </summary>
    public class Job {

        /// <summary>
        /// Descriptive name of the Job. Used for debugging/logging purposes.
        /// </summary>
        public string Name { get; set; } = String.Empty;

        /// <summary>
        /// The URL to the webpage
        /// </summary>
        public string Url { get; set; } = String.Empty;

        /// <summary>
        /// The XPath Query to find the HTML element within the doc
        /// </summary>
        public string Xpath { get; set; } = String.Empty;

        /// <summary>
        /// The job result: Inner HTML of the XPath Query result
        /// </summary>
        public string Result { get; private set; } = String.Empty;

        private readonly HtmlDocument htmlDoc;

        private ILogger? logger;


        public Job() {
            htmlDoc = new HtmlDocument();   //TODO: Check if it is ok to use 1 instance for each job Run
        }

        // This ctor should actually be used if my design was better
        public Job(ILogger logger): this() {            
            SetLogger(logger);
        }

        public void SetLogger(ILogger logger) {
            this.logger = logger;
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
                logger?.Info("Job {0}: No nodes found. Returning.", Name);
                return;
            }            
            logger?.Info("Job {0}: Found {1} matching nodes.", Name, nodes.Count);

            foreach (var node in nodes.ToList()) {                
                logger?.XpathQueryResult(Name, node.InnerHtml);
            }
        }
        
    }
}