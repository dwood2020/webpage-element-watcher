using System;
using System.Reflection;
using Watcher;


var previousColor = Console.ForegroundColor;
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("-------------------------");
Console.WriteLine(" Webpage Element Watcher");
Console.WriteLine(" Version " + Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version);
Console.WriteLine("-------------------------\n");
Console.ForegroundColor = previousColor;

try {
    var app = ApplicationBuilder.BuildFromConfig(@"app.cfg");
    app.Run();
}
catch(Exception e) {
    Console.WriteLine(e);
    Console.WriteLine(e.Message);
}



