#: package Microsoft.Extensions.Hosting@10.0.10
#: package ToolWheel.Extensions.JobManager@1.1.0
#: package ToolWheel.Extensions.JobManager.Conditions@1.1.0

using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ToolWheel;
using ToolWheel.Extensions.JobManager.Services;

// Build and run the host, which will start the background service and listen for user input to trigger the job execution.
Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        // Register the Runtime class as a hosted service, which will run in the background and listen for user input to trigger job execution.
        services.AddHostedService<Runtime>();

        // Register JobManager services and configure the jobs to be managed by the JobManager.
        services.AddJobManager(configure =>
        {
            // Configure the jobs to be managed by the JobManager. In this case, we are adding a job of type TestJobClass with a unique name "UniqueJobName" and specifying the method to be executed when the job is triggered.
            configure.ConfigureJobs(jobs =>
            {
                // Add a job of type TestJobClass with a unique name "UniqueJobName" and specify the method to be executed when the job is triggered. The TaskLimit is set to 1, which means that only one instance of the job can run at a time.
                jobs.Add<TestJobClass>(m => m.TestJob_1)
                    .Id("UniqueJobName")
                    .TaskLimit(1);
            });
        });
    })
    .Build()
    .Run();


// The Runtime class is a background service that listens for user input and triggers the execution of a job when the "Enter" key is pressed. It uses the IJobService to manage and execute jobs.
class Runtime : BackgroundService
{
	private readonly ILogger<Runtime> logger;
    private readonly IJobService jobService;

    // The constructor of the Runtime class takes an IJobService as a parameter, which is used to manage and jobs. The IJobService is injected into the class through dependency injection.
    public Runtime(ILogger<Runtime> logger, IJobService jobService)
    {
		this.logger = logger;
        this.jobService = jobService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Retrieve the jobs by their unique names using the IJobService. The jobs are identified by their unique names "UniqueJobName_1" and "UniqueJobName_2".
        var job = jobService.ReadById("UniqueJobName");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.Enter:
                        // Execute the job when the user presses the "Enter" key. The ExecuteAsync method of the IJobService is called with the job and the cancellation token to execute the job asynchronously.
                        await jobService.ExecuteAsync(job, stoppingToken);
                        break;

                    default:
						logger.LogError("Press Enter on your Keyboard");
                        break;
                }
            }
        }
    }
}

// The TestJobClass is a simple class that contains a method to be executed as a job. It takes an ILogger<TestJobClass> as a dependency through its constructor, which is used to log messages when the job is executed.
class TestJobClass
{
    private ILogger<TestJobClass> logger;

    public TestJobClass(ILogger<TestJobClass> logger)
    {
        this.logger = logger;
    }

    public void TestJob_1()
    {
        logger.LogInformation("TestJob_1 is executing...");
        Thread.Sleep(5000); // Simulate a long-running task by sleeping for 5 seconds.
        logger.LogInformation("TestJob_1 is finished...");
    }
}