using System;
using System.Threading;
using Story.Core;

namespace Story.Examples.Scenarios
{
    public static class BasicScenarios
    {
        public static void RunLoggingExample()
        {
            Console.WriteLine("--- Logging Example ---");
            
            using (var story = Storytelling.StartNew("LoggingExample"))
            {
                Storytelling.Debug("This is a debug message");
                Storytelling.Info("This is an informational message");
                Storytelling.Warn("This is a warning message");
                Storytelling.Error("This is an error message");
                Storytelling.Critical("This is a critical message");
                
                // Demonstrate structured logging
                Storytelling.Info($"User john.doe performed action login at {DateTime.Now}");
            }
        }

        public static void RunDataExample()
        {
            Console.WriteLine("--- Data Example ---");
            
            using (var story = Storytelling.StartNew("DataExample"))
            {
                // Store various types of data
                Storytelling.Data["userId"] = 12345;
                Storytelling.Data["userName"] = "Jane Smith";
                Storytelling.Data["isActive"] = true;
                Storytelling.Data["lastLogin"] = DateTime.Now;
                Storytelling.Data["permissions"] = new[] { "read", "write", "admin" };
                
                Storytelling.Info($"Stored user data: {Storytelling.Data["userName"]}");
                
                // Update data
                Storytelling.Data["lastActivity"] = DateTime.Now;
                Storytelling.Info($"Updated last activity: {Storytelling.Data["lastActivity"]}");
                
                // Demonstrate data retrieval
                var userId = Storytelling.Data["userId"];
                var userName = Storytelling.Data["userName"];
                Storytelling.Info($"Retrieved data - UserId: {userId}, UserName: {userName}");
            }
        }

        public static void RunPerformanceExample()
        {
            Console.WriteLine("--- Performance Example ---");
            
            using (var story = Storytelling.StartNew("PerformanceExample"))
            {
                Storytelling.Info("Starting performance measurement");
                
                // Simulate some work with timing
                var startTime = DateTime.Now;
                
                // Simulate CPU-intensive work
                for (int i = 0; i < 1000000; i++)
                {
                    Math.Sqrt(i);
                }
                
                var endTime = DateTime.Now;
                var duration = endTime - startTime;
                
                Storytelling.Data["operationDuration"] = duration;
                Storytelling.Data["iterations"] = 1000000;
                Storytelling.Data["operationsPerSecond"] = 1000000 / duration.TotalSeconds;
                
                Storytelling.Info("Performance measurement completed");
                Storytelling.Info($"Duration: {duration}, Operations/sec: {Storytelling.Data["operationsPerSecond"]:F2}");
            }
        }

        public static void RunWorkflowExample()
        {
            Console.WriteLine("--- Workflow Example ---");
            
            using (var story = Storytelling.StartNew("WorkflowExample"))
            {
                Storytelling.Info("Starting workflow process");
                
                // Step 1: Initialize
                Storytelling.Info("Step 1: Initializing system");
                Storytelling.Data["step"] = 1;
                Thread.Sleep(100);
                
                // Step 2: Validate
                Storytelling.Info("Step 2: Validating input");
                Storytelling.Data["step"] = 2;
                Storytelling.Data["validationResult"] = "Valid";
                Thread.Sleep(150);
                
                // Step 3: Process
                Storytelling.Info("Step 3: Processing data");
                Storytelling.Data["step"] = 3;
                Storytelling.Data["processedItems"] = 42;
                Thread.Sleep(200);
                
                // Step 4: Finalize
                Storytelling.Info("Step 4: Finalizing results");
                Storytelling.Data["step"] = 4;
                Storytelling.Data["finalResult"] = "Success";
                Thread.Sleep(100);
                
                Storytelling.Info("Workflow completed successfully");
            }
        }
    }
}
