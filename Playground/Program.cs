using System;
using System.Threading.Tasks;
using Story.Core;
using Story.Core.Rules;
using Story.Core.Utils;
using Story.Ext.Handlers;
using Story.Examples.Scenarios;
using Story.Core.Handlers;

namespace Story.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Story Framework Examples ===");
            Console.WriteLine();

            // Test the new instance ID generation
            TestInstanceIdGeneration();

            try
            {
                // Configure StoryLogStore handler
                ConfigureStoryLogStoreHandler();

                // Run all example scenarios
                RunAllScenarios();

                Console.WriteLine();
                Console.WriteLine("=== All Examples Completed Successfully ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error running examples: {0}", ex.Message);
                Console.WriteLine("Stack trace: {0}", ex.StackTrace);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void ConfigureStoryLogStoreHandler()
        {
            try
            {
                Console.WriteLine("Configuring StoryLogStore handler...");

                // Create configuration for StoryLogStore handler
                var config = new StoryLogStoreHandlerConfiguration();

                // Create the handler
                var storyLogStoreHandler = new StoryLogStoreHandler("StoryLogStore", config);
                var handler = new ConsoleHandler("Console", StoryFormatters.GetCompactStoryFormatter(LogSeverity.Debug)).Compose(storyLogStoreHandler);

                // Add the handler to the factory's ruleset
                Storytelling.Factory.GetRuleset().Rules.Add(
                    new PredicateRule(
                        story => story.IsRoot(), // Apply to root stories
                        story => handler));

                Console.WriteLine("StoryLogStore handler configured successfully.");
                Console.WriteLine("API URL: {0}", config.ApiBaseUrl);
                Console.WriteLine("API Key: {0}", string.IsNullOrEmpty(config.ApiKey) || config.ApiKey == "your-api-key-here" ? "[Not configured]" : "[Configured]");
                Console.WriteLine("Batch Size: {0}", config.BatchSize);
                Console.WriteLine("Batch Time Delay: {0}", config.BatchTimeDelay);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Warning: Failed to configure StoryLogStore handler: {0}", ex.Message);
                Console.WriteLine("Continuing with default handlers only...");
                Console.WriteLine();
            }
        }

        private static void RunAllScenarios()
        {
            // Basic scenarios
            BasicScenarios.RunLoggingExample();
            Console.WriteLine();
            
            BasicScenarios.RunDataExample();
            Console.WriteLine();
            
            BasicScenarios.RunPerformanceExample();
            Console.WriteLine();
            
            BasicScenarios.RunWorkflowExample();
            Console.WriteLine();

            // Sub-story scenarios
            SubStoryScenarios.RunSimpleSubStoryExample();
            Console.WriteLine();
            
            SubStoryScenarios.RunNestedSubStoryExample();
            Console.WriteLine();
            
            SubStoryScenarios.RunComplexHierarchyExample();
            Console.WriteLine();
            
            SubStoryScenarios.RunDataFlowExample();
            Console.WriteLine();

            // Error scenarios
            ErrorScenarios.RunExceptionHandlingExample();
            Console.WriteLine();
            
            ErrorScenarios.RunErrorMessageExample();
            Console.WriteLine();
            
            ErrorScenarios.RunSubStoryErrorExample();
            Console.WriteLine();
            
            ErrorScenarios.RunRecoveryExample();
            Console.WriteLine();
            
            ErrorScenarios.RunErrorDataExample();
            Console.WriteLine();

            // Simple async scenarios
            try
            {
                Console.WriteLine("--- Basic Async Example ---");
                RunBasicAsyncExample().Wait();
                Console.WriteLine();
                
                Console.WriteLine("--- Async Sub-Story Example ---");
                RunAsyncSubStoryExample().Wait();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Async scenarios had issues: {0}", ex.Message);
            }
        }

        private static async Task RunBasicAsyncExample()
        {
            using (var story = Storytelling.StartNew("BasicAsyncStory"))
            {
                Storytelling.Info("Starting basic async scenario");
                
                // Simulate async work
                await Task.Delay(200);
                Storytelling.Info("Async work completed");
                
                Storytelling.Data["asyncResult"] = "Basic async operation completed";
                Storytelling.Info($"Async result: {Storytelling.Data["asyncResult"]}");
            }
        }

        private static async Task RunAsyncSubStoryExample()
        {
            using (var mainStory = Storytelling.StartNew("AsyncMainStory"))
            {
                Storytelling.Info("Starting async main story");
                
                // Create async sub-story
                await RunAsyncSubStory("AsyncSubStory1", 150);
                await RunAsyncSubStory("AsyncSubStory2", 100);
                
                Storytelling.Info("All async sub-stories completed");
            }
        }

        private static async Task RunAsyncSubStory(string name, int delayMs)
        {
            using (var subStory = Storytelling.StartNew(name))
            {
                Storytelling.Info($"Starting {name}");
                await Task.Delay(delayMs);
                Storytelling.Info($"Completed {name}");
            }
        }

        static void TestInstanceIdGeneration()
        {
            Console.WriteLine("=== Testing Instance ID Generation ===");
            
            // Test the utility function directly
            var testId1 = InstanceIdGenerator.GenerateInstanceId("TestStory");
            var testId2 = InstanceIdGenerator.GenerateInstanceId("AnotherStory");
            
            Console.WriteLine($"Generated ID 1: {testId1}");
            Console.WriteLine($"Generated ID 2: {testId2}");
            Console.WriteLine($"Format: {{name}}_{{YYMMDD}}_{{NeEwGuid}}");
            Console.WriteLine();
        }
    }
}
