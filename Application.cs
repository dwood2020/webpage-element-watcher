using System;
using Tomlyn;
using Tomlyn.Model;
using System.Threading;


namespace Watcher {

    public class Application {

        public long IntervalSeconds { get; set; } = 60;
        
        public string? DatabasePath { get; set; }

        public User? User { get; set; }
        
        public Logger? Logger { get; set; }

        public List<Job>? Jobs { get; set; }

        private List<Task> jobTasks;

#pragma warning disable CS8618 
        public Application() {
#pragma warning restore CS8618 
            Init();
        }

        public void Init() {
            jobTasks = new List<Task>();

            if (Jobs != null && Logger != null) {                
                foreach (var j in Jobs) {
                    j.SetLogger(Logger);
                }
            }

            // Don't allow website spamming
            if (IntervalSeconds < 60) {
                Logger?.Warning("Application: Interval is too short. Defaulting to 60 seconds.");
                IntervalSeconds = 60;
            }
        }
        

        public void Run() {
            
            Logger?.Info("Application: Length of Jobs List: {0}", Jobs?.Count ?? 0);
            Logger?.Info("Application: Run Interval: {0}", IntervalSeconds);

            if (Jobs == null) {                
                Logger?.Info("Application: No jobs found. Terminating.");
                return;
            }
            
            Logger?.Info("Application: Entering Run Loop");

            while (true) {
                foreach (Job j in Jobs) {
                    jobTasks.Add(j.Run());
                }

                //NOTE: this is a sync method and will pause here until all tasks have completed
                Task.WaitAll(jobTasks.ToArray());

                //TODO: Archive Job results in DB
                
                Thread.Sleep((int)IntervalSeconds * 1000);  //Primitive delay for now
            }
        }    
        
    }
}