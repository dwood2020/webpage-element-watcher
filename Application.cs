using System;
using Tomlyn;
using Tomlyn.Model;


namespace Watcher {

    public class Application {

        public long RunInterval { get; set; }

        public string? DatabasePath { get; set; }

        public User? User { get; set; }

        public List<Job>? Jobs { get; set; }


        public Application() {
            
         }        
        

        public void Run() {

            if (User != null && User.Name != null) {                
                Console.WriteLine("Hello, {0}", User.Name);
            }
            Console.WriteLine("Mail: {0}", User?.Mail);

            Console.WriteLine("DatabasePath: {0}", DatabasePath);

            Console.WriteLine("Length of Jobs List: {0}", Jobs?.Count);

            if (Jobs != null) {
                foreach (Job j in Jobs) {
                    Console.WriteLine("Job URL: {0}", j.Url);
                    //TODO: Generate list of tasks to await all
                }
            }
            
            //Job j = new();
            //await j.Run();
        }        
    }
}