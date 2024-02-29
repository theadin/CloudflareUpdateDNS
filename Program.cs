using CloudflareUpdateDNS;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Log.txt",
        rollingInterval: RollingInterval.Day, // You can adjust the rolling interval
        fileSizeLimitBytes: 500_000,      // 0.5 megabyte
        retainedFileCountLimit: 5)           // Number of files to keep before rolling
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
