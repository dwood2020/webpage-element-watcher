using System;
using Tomlyn;
using Tomlyn.Model;


namespace Watcher {

    public class Application {

        public long IntervalSeconds { get; set; }

        public string? DatabasePath { get; set; }

        public User? User { get; set; }

        public List<Job>? Jobs { get; set; }

        private List<Task> jobTasks;


        public Application() {
            jobTasks = new List<Task>();            
         }        
        

        public void Run() {

            if (User != null && User.Name != null) {                
                Console.WriteLine("Hello, {0}", User.Name);
            }
            Console.WriteLine("Mail: {0}", User?.Mail);

            Console.WriteLine("DatabasePath: {0}", DatabasePath);

            Console.WriteLine("Length of Jobs List: {0}", Jobs?.Count);

            if (Jobs == null) {
                return;
            }

            foreach (Job j in Jobs) {
                jobTasks.Add(j.Run());
            }

            //NOTE: Do the tasks actually run in parallel here?
            Task.WaitAll(jobTasks.ToArray());

        }        
    }
}