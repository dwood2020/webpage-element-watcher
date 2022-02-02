
namespace Watcher {


    public class Job {
        
        public string? Url { get; set; }

        public Job() { }


        public async Task Run() {

            // this can be done in async method, see
            // here: https://stackoverflow.com/questions/25055749/terminate-or-exit-c-sharp-async-method-with-return
            if (Url == null) {
                return;
            }

            string content = await WebClient.GetInstance().GetHtml(Url);
            Console.WriteLine(content);
        }
        
    }
}