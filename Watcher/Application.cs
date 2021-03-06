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
        /// Simple mail client to be used for notifications (for now)
        /// </summary>
        public IMailClient MailClient { get; private set; }

        /// <summary>
        /// All jobs to be performed. Constructed via CFG input.
        /// NOTE: INjecting the logger dependency into the jobs requires a separate call
        /// to Application.Init() after construction.
        /// </summary>
        public List<IJob> Jobs { get; set; }

        // All job tasks are held here
        private List<Task> jobTasks;

        public Application(ILogger logger, IDatabase database, IUser user, IMailClient mailClient, List<IJob> jobs) {
            Logger = logger;
            Database = database;
            User = user;
            MailClient = mailClient;
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
                try {
                    Task.WaitAll(jobTasks.ToArray());
                }
                catch {
                    Logger.Error("Job task failed.");
                }
                
                jobTasks.Clear();

                foreach (IJob j in Jobs) {
                    if (j.Result != null) {
                        Database.InsertJobResult(j.Name, j.Result);
                    }
                    else {
                        Logger.Warning("Application: Job \"{0}\" Result is null.", j.Name);
                    }
                }

                // keep yielding and comparing results in separate loops
                foreach (IJob j in Jobs) {
                    // for now we skip Jobs whos last result was null
                    // as the did not contribute a new db entry and any difference would have been detected earlier
                    if (j.Result == null) {
                        continue;
                    }
                    
                    List<JobResult> lastJobs = Database.GetLastJobResults(j.Name, 2);
                    if (lastJobs.Count == 2 && !lastJobs[0].IsEqual(lastJobs[1])) {
                        // something has changed, notify
                        SendHasChangedNotification(j, lastJobs);
                    }

                }

                Thread.Sleep((int)IntervalSeconds * 1000);  //Primitive delay for now - this MUST change when this app receives messages
            }
        }


        private void SendHasChangedNotification(IJob job, List<JobResult> resultsDiff) {
            Logger.Info("NOTIFICATION: Job \"{0}\": Content has changed", job.Name);

            string subject = "Change notification";
            string msg = String.Format("Hi {0}! \nChange notification for job \"{1}\":\n\n", User.Name, job.Name);
            msg += String.Format("New record: {0}\nOld record: {1}\n", resultsDiff[0].ToString(), resultsDiff[1].ToString());
            msg += "\nRegards :)";

            MailClient.SendMessage(subject, msg);
        }
        
    }
}
