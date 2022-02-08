using HtmlAgilityPack;
using System.Text;


namespace Watcher {

    //TODO: Comment this interface
    public interface IJob<T> {
        public string Name { get; set; }
        public string Url { get; set; }
        public string XPath { get; set; }
        public JobResult<T> Result { get; protected set; }
        public Task Run();
    }


    /// <summary>
    /// This class represents a Job result which consists of a job execution timestamp and the result value.
    /// </summary>
    /// <typeparam name="T">Result type: string (default) or double (called "number")</typeparam>
    public class JobResult<T> {

        public string Timestamp { get; set; }
        public T Content { get; set; }

        public JobResult(string timestamp, T content) {
            Timestamp = timestamp;
            Content = content;
        }

        public override string ToString() {
            return "Timestamp: " + Timestamp + "  Content: " + Content;
        }
    }


    /// <summary>
    /// This class represents a watcher job.
    /// </summary>
    public class Job {

        // NOTE: These simple strings are used instead of a proper enum to make TOML parsing easy and automatic
        public static readonly string ResultTypeNumber = "number";
        public static readonly string ResultTypeString = "string";

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

        public string ResultType { get; set; } = ResultTypeString;

        /// <summary>
        /// The job result: Inner HTML of the XPath Query result as String.
        /// (This is the default)
        /// Set to Null if no result found.
        /// </summary>
        public JobResult<string>? StringResult { get; private set; }

        /// <summary>
        /// The job result: Inner HTML of the XPath Query result as Number(double).
        /// (This is the default)
        /// Set to Null if no result found.
        /// </summary>
        public JobResult<double>? NumberResult { get; private set; }

        // every Job has its own HTML document as they can be generally different in each job
        private readonly HtmlDocument htmlDoc;

        private ILogger? logger;


        public Job() {
            htmlDoc = new HtmlDocument();   //TODO: Check if it is ok to use 1 instance for each job Run
            StringResult = null;
            NumberResult = null;
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
            StringResult = null;
            NumberResult = null;

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

            // timestamp of job execution
            string timestamp = DateTime.Now.ToString("G");

            if (ResultType.ToLower() == ResultTypeNumber) {
                double parseResult = 0.0;
                string s = PreprocessNumberString(nodes[0].InnerText);
                if (!Double.TryParse(s, out parseResult)) {
                    logger?.Error("Job {0}: Number result type could not be parsed. Setting 0", Name);
                }
                NumberResult = new JobResult<double>(timestamp, parseResult);
                logger?.XpathQueryResult(Name, NumberResult.ToString());
            }
            else {
                StringResult = new JobResult<string>(timestamp, nodes[0].InnerText);
                logger?.XpathQueryResult(Name, StringResult.ToString());
            }
        }
        
        private static string PreprocessNumberString(string s) {
            StringBuilder sb = new();

            foreach (char c in s) {
                if (c >= '0' && c <= '9' || c == ',' || c == '.') {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}