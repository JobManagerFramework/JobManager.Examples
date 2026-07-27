#: package Microsoft.Extensions.Hosting@10.0.10
#: package ToolWheel.Extensions.JobManager@1.1.0

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
                // Add a job of type TestJobClass with a unique name "UniqueJobName" and specify the method to be executed when the job is triggered.
                jobs.Add<TestJobClass>(m => m.TestJob_1).Id("UniqueJobName_1");

                // Add a job using a lambda expression that logs a warning message when executed, and assign it a unique name "UniqueJobName_2".
                jobs.Add((ILogger logger) => { logger.LogWarning("Hello World from Job 2"); }).Id("UniqueJobName_2");
            });
        });
    })
    .Build()
    .Run();


// The Runtime class is a background service that listens for user input to trigger the execution of jobs managed by the JobManager. It retrieves the jobs by their unique names and executes them when the corresponding key is pressed.
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
        var job_1 = jobService.ReadById("UniqueJobName_1");
        var job_2 = jobService.ReadById("UniqueJobName_2");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        // Execute the first job when the user presses the "1" key or the "NumPad1" key. The ExecuteAsync method of the IJobService is called with the job and the cancellation token to execute the job asynchronously.
                        await jobService.ExecuteAsync(job_1, stoppingToken);
                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        // Execute the second job when the user presses the "2" key or the "NumPad2" key. The ExecuteAsync method of the IJobService is called with the job and the cancellation token to execute the job asynchronously.
                        await jobService.ExecuteAsync(job_2, stoppingToken);
                        break;
                    default:
						logger.LogError("Press 1 or 2 on your Keyboard");
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
        logger.LogWarning("Hello World from Job 1");
    }
}