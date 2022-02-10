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
    public sealed class Application {

        /// <summary>
        /// Pause interval between two job runs.
        /// Min. should be 60 seconds to avoid website spamming.
        /// </summary>
        public long IntervalSeconds { get; set; } = 60;
        
        /// <summary>
        /// Application user. Constructed via CFG input.
        /// </summary>
        public IUser User { get; private set; }
        
        /// <summary>
        /// Logger. Constructed via CFG input.
        /// </summary>
        public ILogger Logger { get; private set; }

        /// <summary>
        /// Database. Constructed via CFG input.
        /// </summary>
        public IDatabase Database { get; private set; }

        /// <summary>
        /// All jobs to be performed. Constructed via CFG input.
        /// NOTE: INjecting the logger dependency into the jobs requires a separate call
        /// to Application.Init() after construction.
        /// </summary>
        public List<Job> Jobs { get; set; }

        // All job tasks are held here
        private List<Task> jobTasks;

        public Application(ILogger logger, IDatabase database, IUser user, List<Job> jobs) {
            Logger = logger;
            Database = database;
            User = user;
            Jobs = jobs;
            jobTasks = new List<Task>();
        }

        
        /// <summary>
        /// Runs the application and enters the worker loop.
        /// </summary>
        public void Run() {

            Logger.Info("Application: Length of Jobs List: {0}", Jobs.Count);
            Logger.Info("Application: Run Interval: {0} Seconds", IntervalSeconds);

            if (Jobs.Count == 0) {                
                Logger.Info("Application: No jobs found. Terminating.");
                return;
            }
            
            Logger.Info("Application: Entering Run Loop");

            while (true) {
                foreach (Job j in Jobs) {
                    jobTasks.Add(j.Run());
                }

                //NOTE: this is a sync method and will pause here until all tasks have completed
                Task.WaitAll(jobTasks.ToArray());

                foreach (Job j in Jobs) { 
                    if (j is NumberJob) {
                        NumberJob nj = (NumberJob)j;
                        if (nj.Result != null) {
                            Database.InsertJobResult(nj.Name, nj.Result);
                        }
                        else {
                            Logger.Warning("Application: NumberJob \"{0}\" Result is null.", nj.Name);
                        }
                        
                    }
                    else if (j is StringJob) {
                        StringJob sj = (StringJob)j;
                        if (sj.Result != null) {
                            Database.InsertJobResult(sj.Name, sj.Result);
                        }
                        else {
                            Logger.Warning("Application: StringJob \"{0}\" Result is null.", sj.Name);
                        }
                    }
                    else {
                        Logger.Error("Application: Something went wrong on DB insert for job {0}", j.Name);
                    }
                }

                Thread.Sleep((int)IntervalSeconds * 1000);  //Primitive delay for now - this MUST change when this app receives messages
            }
        }    
        
    }
}