using System;
using Tomlyn;
using Tomlyn.Model;


namespace Watcher {

    public class Application {

        public long RunInterval { get; set; }

        public User? User { get; set; }

        public Application() { }
        

        public void Run() {

            if (User != null && User.Name != null) {                
                Console.WriteLine("Hello, {0}", User.Name);
            }
            Console.WriteLine("Mail: {0}", User?.Mail);
        }
    }
}