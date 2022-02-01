using System;

public class Application {

    //TODO Get rid of this boolean later
    private bool isConfigured = false;

    public long RunInterval { get; set; }

    public Application() { }


    public void Configure(string configFilePath) {

        // on correct config
        isConfigured = true;
    }

    public void Run() {
        if (!isConfigured) {
            throw new InvalidOperationException("Application instance not configured. Call Configure() first!");
        }
        Console.WriteLine("Hello, World!");
    }
}