
namespace Watcher {


    public class Job {
        
        public string? Url { get; set; }

        public Job() { }


        public async Task Run() {

            string content = await WebClient.GetInstance().GetHtml("https://www.google.de/");
            Console.WriteLine(content);
        }
        
    }
}