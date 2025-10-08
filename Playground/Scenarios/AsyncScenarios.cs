using System;
using System.Threading;
using System.Threading.Tasks;
using Story.Core;

namespace Story.Examples.Scenarios
{
    public static class AsyncScenarios
    {
        public static async Task RunBasicAsyncExample()
        {
            Console.WriteLine("--- Basic Async Example ---");
            
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

        public static async Task RunAsyncSubStoryExample()
        {
            Console.WriteLine("--- Async Sub-Story Example ---");
            
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

        public static async Task RunParallelAsyncExample()
        {
            Console.WriteLine("--- Parallel Async Example ---");
            
            using (var mainStory = Storytelling.StartNew("ParallelAsyncStory"))
            {
                Storytelling.Info("Starting parallel async scenario");
                
                // Start multiple async operations in parallel
                var task1 = RunAsyncOperation("Operation1", 200);
                var task2 = RunAsyncOperation("Operation2", 150);
                var task3 = RunAsyncOperation("Operation3", 100);
                
                // Wait for all to complete
                await Task.WhenAll(task1, task2, task3);
                
                Storytelling.Info("All parallel async operations completed");
            }
        }

        private static async Task RunAsyncOperation(string name, int delayMs)
        {
            using (var story = Storytelling.StartNew(name))
            {
                Storytelling.Info($"Starting {name}");
                await Task.Delay(delayMs);
                Storytelling.Info($"Completed {name}");
            }
        }

        public static async Task RunAsyncErrorExample()
        {
            Console.WriteLine("--- Async Error Example ---");
            
            using (var story = Storytelling.StartNew("AsyncErrorStory"))
            {
                Storytelling.Info("Starting async error scenario");
                
                try
                {
                    await SimulateAsyncFailure();
                }
                catch (Exception ex)
                {
                    Storytelling.Error($"Async operation failed: {ex.Message}");
                    Storytelling.ReportFailed(ex);
                }
                
                Storytelling.Info("Async error scenario completed");
            }
        }

        private static async Task SimulateAsyncFailure()
        {
            await Task.Delay(100);
            throw new InvalidOperationException("Simulated async failure");
        }

        public static async Task RunAsyncDataFlowExample()
        {
            Console.WriteLine("--- Async Data Flow Example ---");
            
            using (var mainStory = Storytelling.StartNew("AsyncDataFlowStory"))
            {
                Storytelling.Info("Starting async data flow scenario");
                Storytelling.Data["initialData"] = "Initial data";
                
                // Async operation 1
                var result1 = await ProcessDataAsync("Data1", 100);
                Storytelling.Data["result1"] = result1;
                
                // Async operation 2
                var result2 = await ProcessDataAsync("Data2", 150);
                Storytelling.Data["result2"] = result2;
                
                // Combine results
                var combinedResult = $"{Storytelling.Data["result1"]} + {Storytelling.Data["result2"]}";
                Storytelling.Data["combinedResult"] = combinedResult;
                
                Storytelling.Info($"Data flow completed. Combined result: {combinedResult}");
            }
        }

        private static async Task<string> ProcessDataAsync(string data, int delayMs)
        {
            using (var story = Storytelling.StartNew("Process" + data))
            {
                Storytelling.Info($"Processing {data}");
                await Task.Delay(delayMs);
                var result = data + "_processed";
                Storytelling.Info($"Processed {data} -> {result}");
                return result;
            }
        }

        public static async Task RunAsyncCancellationExample()
        {
            Console.WriteLine("--- Async Cancellation Example ---");
            
            using (var story = Storytelling.StartNew("AsyncCancellationStory"))
            {
                Storytelling.Info("Starting async cancellation scenario");
                
                var cancellationToken = new CancellationTokenSource();
                
                try
                {
                    // Start a long-running operation
                    var longTask = LongRunningOperation(cancellationToken.Token);
                    
                    // Cancel after 500ms
                    await Task.Delay(500);
                    cancellationToken.Cancel();
                    
                    await longTask;
                }
                catch (OperationCanceledException)
                {
                    Storytelling.Warn("Operation was cancelled");
                    Storytelling.ReportFailed("Operation cancelled by user");
                }
                
                Storytelling.Info("Async cancellation scenario completed");
            }
        }

        private static async Task LongRunningOperation(CancellationToken cancellationToken)
        {
            using (var story = Storytelling.StartNew("LongRunningOperation"))
            {
                Storytelling.Info("Starting long-running operation");
                
                for (int i = 0; i < 10; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    Storytelling.Info($"Operation progress: {i + 1}/10");
                    await Task.Delay(200, cancellationToken);
                }
                
                Storytelling.Info("Long-running operation completed");
            }
        }

        public static async Task RunAsyncNestedExample()
        {
            Console.WriteLine("--- Async Nested Example ---");
            
            using (var level1Story = Storytelling.StartNew("AsyncLevel1"))
            {
                Storytelling.Info("Starting async level 1");
                
                await Task.Delay(50);
                
                using (var level2Story = Storytelling.StartNew("AsyncLevel2"))
                {
                    Storytelling.Info("Starting async level 2");
                    
                    await Task.Delay(50);
                    
                    using (var level3Story = Storytelling.StartNew("AsyncLevel3"))
                    {
                        Storytelling.Info("Starting async level 3");
                        await Task.Delay(50);
                        Storytelling.Info("Completed async level 3");
                    }
                    
                    Storytelling.Info("Completed async level 2");
                }
                
                Storytelling.Info("Completed async level 1");
            }
        }
    }
}
