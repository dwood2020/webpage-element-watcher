using HtmlAgilityPack;
using System.Text;


namespace Watcher {

    //TODO: Comment this interface
    //NOTE: I do not yet understand how I would decouple these job classes for Unit Testing.
    // For now, I will rely on these interfaces and see later when I actually implement the tests
    public interface IJob<T> {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Xpath { get; set; }

        public JobResult<T>? Result { get; }

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
    public abstract class Job {

        /// <summary>
        /// Descriptive name of the Job. Used for debugging/logging purposes.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The URL to the webpage
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The XPath Query to find the HTML element within the doc
        /// </summary>
        public string Xpath { get; set; }

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
            Xpath = String.Empty;
        }

        /// <summary>
        /// Run the Job (Async method)
        /// </summary>
        /// <returns>Async task</returns>
        public async Task Run() {

            // this can be done in async method, see
            // here: https://stackoverflow.com/questions/25055749/terminate-or-exit-c-sharp-async-method-with-return
            if (Url.Length == 0 || Xpath.Length == 0) {
                return;
            }

            string html = await webClient.GetHtml(Url);

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

        protected abstract void PrepareResult(string timestamp, string htmlResult);

    }

    //TODO: COmment
    public class StringJob : Job, IJob<string> {

        public JobResult<string>? Result { get; private set; }

        public StringJob(ILogger logger, IWebClient webClient) : base(logger, webClient) { }

        protected override void PrepareResult(string timestamp, string htmlResult) {
            Result = new JobResult<string>(timestamp, htmlResult);
            logger.XpathQueryResult(Name, Result.ToString());
        }
    }

    //TODO: Comment
    public class NumberJob : Job, IJob<double> {

        public JobResult<double>? Result { get; private set; }

        public NumberJob(ILogger logger, IWebClient webClient) : base(logger, webClient) { }

        protected override void PrepareResult(string timestamp, string htmlResult) {
            double parseResult = 0.0;
            string s = PreprocessNumberString(htmlResult);
            if (!Double.TryParse(s, out parseResult)) {
                logger.Error("Job {0}: Number result type could not be parsed. Setting 0", Name);
            }
            Result = new JobResult<double>(timestamp, parseResult);
            logger.XpathQueryResult(Name, Result.ToString());
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