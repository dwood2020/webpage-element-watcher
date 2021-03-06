using HtmlAgilityPack;
using System.Text;
using System.IO;


namespace Watcher {

    /// <summary>
    /// This is the general Job interface
    /// </summary>
    public interface IJob {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Xpath { get; set; }

        public JobResult? Result { get; }

        public Task Run();
    }


    /// <summary>
    /// This class represents a Job result which consists of a job execution timestamp and the result value.
    /// </summary>    
    public class JobResult {

        public string Timestamp { get; set; }
        public string Content { get; set; }

        public JobResult(string timestamp, string content) {
            Timestamp = timestamp;
            Content = content;
        }

        public bool IsEqual(JobResult other) {
            return string.Compare(this.Content, other.Content, StringComparison.Ordinal) == 0;
        }

        public override string ToString() {
            return "Timestamp: " + Timestamp + "  Content: " + Content;
        }
    }


    /// <summary>
    /// This class represents a watcher job.
    /// </summary>
    public class Job : IJob {

        /// <summary>
        /// Descriptive name of the Job. Used for debugging/logging purposes.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The URL to the webpage
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Path to a local HTML file as alternative to web URL
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// The XPath Query to find the HTML element within the doc
        /// </summary>
        public string Xpath { get; set; }

        /// <summary>
        /// For now: Set this flag if the result shall be treated as number
        /// (This only involves string preprocessing internally)
        /// </summary>
        public bool TreatAsNumber { get; set; }        

        /// <summary>
        /// The last Job result. 
        /// Is null if no result / last Job run failed.
        /// </summary>
        public JobResult? Result { get; protected set; }

        protected readonly ILogger logger;

        protected readonly IWebClient webClient;

        // every Job has its own HTML document as they can be generally different in each job
        protected readonly HtmlDocument htmlDoc;


        public Job(ILogger logger, IWebClient webClient) {
            this.logger = logger;
            this.webClient = webClient;
            htmlDoc = new HtmlDocument();

            Name = String.Empty;
            Url = String.Empty;
            LocalPath = String.Empty;
            Xpath = String.Empty;
            TreatAsNumber = false;            
        }

        /// <summary>
        /// Run the Job (Async method)
        /// </summary>
        /// <returns>Async task</returns>
        public async Task Run() {

            // this can be done in async method, see
            // here: https://stackoverflow.com/questions/25055749/terminate-or-exit-c-sharp-async-method-with-return
            if (Url.Length == 0 && LocalPath.Length == 0 || Xpath.Length == 0) {
                Result = null;
                return;
            }

            string html;
            if (Url.Length > 0) {
                html = await webClient.GetHtml(Url);
            }
            else if (LocalPath.Length > 0) {
                // do not throw an exception here if file is not found - this is not critical
                try {
                    html = await File.ReadAllTextAsync(LocalPath);
                }
                catch {
                    logger.Error("Job \"{0}\": Could not open local file \"{1}\"", Name, LocalPath);
                    html = string.Empty;
                }
            } 
            else {
                // this should not happen
                html = string.Empty;
            }

            // handle the empty string here in one place
            if (html.Length == 0) {
                Result = null;
                return;
            }

            htmlDoc.LoadHtml(html);
            // simply utilise XPath Sysntax here, see
            // https://www.w3schools.com/xml/xpath_syntax.asp
            var nodes = htmlDoc.DocumentNode.SelectNodes(Xpath);

            if (nodes == null || nodes.Count == 0) {
                logger?.Info("Job {0}: No nodes found. Returning.", Name);
                return;
            }
            logger.Info("Job {0}: Found {1} matching nodes.", Name, nodes.Count);

            if (nodes.Count > 1) {
                logger.Warning("Job {0}: Using first node. This may be incorrect", Name);
            }

            // timestamp of job execution
            string timestamp = DateTime.Now.ToString("G");

            // call the abstract method here
            PrepareResult(timestamp, nodes[0].InnerText);
        }

        protected void PrepareResult(string timestamp, string htmlResult) {
            string result;

            if (TreatAsNumber) {
                result = PreprocessNumberString(htmlResult);
            }
            else {
                result = PreprocessString(htmlResult);
            }
            
            Result = new JobResult(timestamp, result);
            logger.XpathQueryResult(Name, Result.ToString());
        }

        protected string PreprocessString(string s) {
            StringBuilder sb = new();

            foreach (char c in s) {                
                //TODO: Improve - maybe use a Regex?
                if (c <= sbyte.MaxValue && c != '\n' && c != '\r') {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        protected string PreprocessNumberString(string s) {
            StringBuilder sb = new();

            foreach (char c in s) {
                if (c >= '0' && c <= '9' || c == '.' || c == ',') {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

    }

}