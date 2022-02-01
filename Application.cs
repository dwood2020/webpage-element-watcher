using System;
using Tomlyn;
using Tomlyn.Model;


namespace Watcher {

    public class Application {

        public long RunInterval { get; set; }

        public User? User { get; set; }

        public Application() { }
        

        public void Run() {

            Console.WriteLine("Hello, World!");
        }
    }
}