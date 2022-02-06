using System;
using Tomlyn;
using Tomlyn.Model;
using System.Threading;


namespace Watcher {

    /// <summary>
    /// This class represents the actual Watcher application.
    /// It holds all objects and methods which are part of the application's purpose.
    /// It is constructed via a stateless factory directly from the CFG input.
    /// </summary>
    public class Application {

        /// <summary>
        /// Pause interval between two job runs.
        /// Min. should be 60 seconds to avoid website spamming.
        /// </summary>
        public long IntervalSeconds { get; set; } = 60;
        
        /// <summary>
        /// File path to the database file
        /// </summary>
        public string? DatabasePath { get; set; }

        /// <summary>
        /// Application user. Constructed via CFG input.
        /// </summary>
        public User? User { get; set; }
        
        /// <summary>
        /// Logger. Constructed via CFG input.
        /// </summary>
        public Logger? Logger { get; set; }

        /// <summary>
        /// All jobs to be performed. Constructed via CFG input.
        /// NOTE: INjecting the logger dependency into the jobs requires a separate call
        /// to Application.Init() after construction.
        /// </summary>
        public List<Job>? Jobs { get; set; }

        // All job tasks are held here
        private List<Task> jobTasks;

        public Application() {
            jobTasks = new List<Task>();
        }

        /// <summary>
        /// Init the associated Jobs by injecting the logger dependency.
        /// This is a separate method as the ctor runs before the attributes are set 
        /// during construction in factory.
        /// </summary>
        public void Init() {
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
        
        /// <summary>
        /// Runs the application and enters the worker loop.
        /// </summary>
        public void Run() {
            
            Logger?.Info("Application: Length of Jobs List: {0}", Jobs?.Count ?? 0);
            Logger?.Info("Application: Run Interval: {0} Seconds", IntervalSeconds);

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
                
                Thread.Sleep((int)IntervalSeconds * 1000);  //Primitive delay for now - this MUST change when this app receives messages
            }
        }    
        
    }
}