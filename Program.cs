using System;
using Watcher;

var previousColor = Console.ForegroundColor;
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Webpage Element Watcher 0.0.1");
Console.WriteLine("-----------------------------\n");
Console.ForegroundColor = previousColor;

try {
    var app = ApplicationBuilder.BuildFromConfig(@"app.cfg");
    app.Run();
}
catch(Exception e) {
    Console.WriteLine(e);
    Console.WriteLine(e.Message);
}



