using Watcher;


var app = ApplicationBuilder.BuildFromConfig(@"app.cfg");

app.Configure(@"app.cfg");
app.Run();

