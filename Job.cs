using HtmlAgilityPack;


namespace Watcher {

    public class Job {
        
        public string? Url { get; set; }

        private HtmlDocument htmlDoc;

        public Job() {
            htmlDoc = new HtmlDocument();   //TODO: Check if it is ok to use 1 instance for each job Run
         }


        public async Task Run() {

            // this can be done in async method, see
            // here: https://stackoverflow.com/questions/25055749/terminate-or-exit-c-sharp-async-method-with-return
            if (Url == null) {
                return;
            }

            string html = await WebClient.GetInstance().GetHtml(Url);

            htmlDoc.LoadHtml(html);
            // simply utilise XPath Sysntax here, see
            // https://www.w3schools.com/xml/xpath_syntax.asp
            var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='counter']");

            if (nodes == null) {
                Console.WriteLine("No nodes found");
                return;
            }
            Console.WriteLine("Found {0} matching nodes.", nodes.Count);
            foreach (var node in nodes.ToList()) {
                Console.WriteLine("Matching HTML Node Content: {0}", node.InnerHtml);
            }
        }
        
    }
}