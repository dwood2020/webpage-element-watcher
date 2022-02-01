using System;
using Watcher;


try {
    var app = ApplicationBuilder.BuildFromConfig(@"app.cfg");
    app.Run();
}
catch(Exception e) {
    Console.WriteLine(e.Message);
}



