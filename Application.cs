using System;
using Tomlyn;
using Tomlyn.Model;
using System.Threading;


namespace Watcher {

    public class Application {

        public long IntervalSeconds { get; set; }

        public string? DatabasePath { get; set; }

        public User? User { get; set; }

        public List<Job>? Jobs { get; set; }

        //TODO: Logger should be owned by Application here! Change this
        public bool VerboseOutput {
            get { return Logger.GetInstance().Verbose; }
            set { Logger.GetInstance().Verbose = value; }
        }

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
                Console.WriteLine("No jobs found. Terminating");
                return;
            }

            Console.WriteLine("Entering Run Loop");

            while (true) {
                foreach (Job j in Jobs) {
                    jobTasks.Add(j.Run());
                }

                Task.WaitAll(jobTasks.ToArray());
                //NOTE: this is a sync method and will pause here until all tasks have completed

                //TODO: Archive Job results in DB

                Thread.Sleep((int)IntervalSeconds * 1000);  //Primitive delay for now
            }
        }    
        
    }
}