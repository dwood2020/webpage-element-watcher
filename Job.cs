using HtmlAgilityPack;


namespace Watcher {

    /// <summary>
    /// This class represents a watcher job.
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
        /// The job result: Inner HTML of the XPath Query result.
        /// Set to Null if no result found.
        /// </summary>
        public string? Result { get; private set; }

        // every Job has its own HTML document as they can be generally different in each job
        private readonly HtmlDocument htmlDoc;

        private ILogger? logger;


        public Job() {
            htmlDoc = new HtmlDocument();   //TODO: Check if it is ok to use 1 instance for each job Run
        }

        // This ctor should actually be used if my design was better
        public Job(ILogger logger): this() {            
            SetLogger(logger);
        }

        /// <summary>
        /// Sets the logger dependency.
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public void SetLogger(ILogger logger) {
            this.logger = logger;
        }

        /// <summary>
        /// Run the Job (Async method)
        /// </summary>
        /// <returns>Async task</returns>
        public async Task Run() {

            // set Result to null on every run first
            Result = null;

            // this can be done in async method, see
            // here: https://stackoverflow.com/questions/25055749/terminate-or-exit-c-sharp-async-method-with-return
            if (Url.Length == 0 || Xpath.Length == 0) {
                return;
            }

            string html = await WebClient.GetInstance().GetHtml(Url);

            htmlDoc.LoadHtml(html);
            // simply utilise XPath Sysntax here, see
            // https://www.w3schools.com/xml/xpath_syntax.asp
            var nodes = htmlDoc.DocumentNode.SelectNodes(Xpath);

            if (nodes == null || nodes.Count == 0) {                
                logger?.Info("Job {0}: No nodes found. Returning.", Name);
                return;
            }            
            logger?.Info("Job {0}: Found {1} matching nodes.", Name, nodes.Count);

            if (nodes.Count > 1) {
                logger?.Warning("Job {0}: Using first node. This may be incorrect", Name);
            }

            Result = nodes[0].InnerHtml;
            logger?.XpathQueryResult(Name, Result);            
        }
        
    }
}