using System;
using Tomlyn;
using Tomlyn.Model;

public class Application {

    public long RunInterval { get; set; }

    public User? User { get; set; }

    public Application() { }


    public void Configure(string configFilePath) {
        
    }

    public void Run() {
        
        Console.WriteLine("Hello, World!");
    }
}