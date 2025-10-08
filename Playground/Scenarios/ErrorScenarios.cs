using System;
using System.Threading;
using Story.Core;

namespace Story.Examples.Scenarios
{
    public static class ErrorScenarios
    {
        public static void RunExceptionHandlingExample()
        {
            Console.WriteLine("--- Exception Handling Example ---");
            
            try
            {
                using (var story = Storytelling.StartNew("ExceptionHandlingStory"))
                {
                    Storytelling.Info("Starting exception handling scenario");
                    
                    try
                    {
                        // Simulate an operation that might fail
                        SimulateRiskyOperation();
                    }
                    catch (Exception ex)
                    {
                        Storytelling.Error($"Operation failed with exception: {ex.Message}");
                        Storytelling.ReportFailed(ex);
                        throw; // Re-throw to demonstrate error propagation
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught at top level: {ex.Message}");
            }
        }

        public static void RunErrorMessageExample()
        {
            Console.WriteLine("--- Error Message Example ---");
            
            using (var story = Storytelling.StartNew("ErrorMessageStory"))
            {
                Storytelling.Info("Starting error message scenario");
                
                // Simulate a validation failure
                var isValid = ValidateInput("invalid input");
                if (!isValid)
                {
                    Storytelling.Error("Input validation failed");
                    Storytelling.ReportFailed("Invalid input provided: validation failed");
                }
                
                Storytelling.Info("Error message scenario completed");
            }
        }

        public static void RunSubStoryErrorExample()
        {
            Console.WriteLine("--- Sub-Story Error Example ---");
            
            using (var mainStory = Storytelling.StartNew("MainStoryWithErrors"))
            {
                Storytelling.Info("Starting main story with potential sub-story errors");
                
                // Sub-story that succeeds
                using (var successStory = Storytelling.StartNew("SuccessSubStory"))
                {
                    Storytelling.Info("This sub-story will succeed");
                    Thread.Sleep(50);
                    Storytelling.Info("Success sub-story completed");
                }
                
                // Sub-story that fails
                try
                {
                    using (var failureStory = Storytelling.StartNew("FailureSubStory"))
                    {
                        Storytelling.Info("This sub-story will fail");
                        Storytelling.ReportFailed("Simulated failure in sub-story");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Sub-story failed as expected: {0}", ex.Message);
                }
                
                Storytelling.Info("Main story completed despite sub-story failure");
            }
        }

        public static void RunRecoveryExample()
        {
            Console.WriteLine("--- Error Recovery Example ---");
            
            using (var story = Storytelling.StartNew("RecoveryStory"))
            {
                Storytelling.Info("Starting recovery scenario");
                
                var attempts = 0;
                var maxAttempts = 3;
                var success = false;
                
                while (attempts < maxAttempts && !success)
                {
                    attempts++;
                    Storytelling.Info($"Attempt {attempts} of {maxAttempts}");
                    
                    try
                    {
                        // Simulate an operation that might fail
                        SimulateUnreliableOperation();
                        success = true;
                        Storytelling.Info($"Operation succeeded on attempt {attempts}");
                    }
                    catch (Exception ex)
                    {
                        Storytelling.Warn($"Attempt {attempts} failed: {ex.Message}");
                        
                        if (attempts < maxAttempts)
                        {
                            Storytelling.Info("Retrying in 100ms...");
                            Thread.Sleep(100);
                        }
                        else
                        {
                            Storytelling.Error("All attempts failed");
                            Storytelling.ReportFailed(ex);
                        }
                    }
                }
                
                if (success)
                {
                    Storytelling.Info("Recovery scenario completed successfully");
                }
                else
                {
                    Storytelling.Info("Recovery scenario failed after all attempts");
                }
            }
        }

        public static void RunErrorDataExample()
        {
            Console.WriteLine("--- Error Data Example ---");
            
            using (var story = Storytelling.StartNew("ErrorDataStory"))
            {
                Storytelling.Info("Starting error data scenario");
                
                try
                {
                    // Simulate an operation that fails
                    SimulateDataOperation();
                }
                catch (Exception ex)
                {
                    Storytelling.Error($"Data operation failed: {ex.Message}");
                    
                    // Store error details in story data
                    Storytelling.Data["errorType"] = ex.GetType().Name;
                    Storytelling.Data["errorMessage"] = ex.Message;
                    Storytelling.Data["errorTimestamp"] = DateTime.Now;
                    Storytelling.Data["errorSource"] = "SimulateDataOperation";
                    
                    Storytelling.ReportFailed(ex);
                    
                    // Log the error data
                    Storytelling.Info($"Error details stored: Type={Storytelling.Data["errorType"]}, Message={Storytelling.Data["errorMessage"]}, Timestamp={Storytelling.Data["errorTimestamp"]}");
                }
            }
        }

        private static void SimulateRiskyOperation()
        {
            // Simulate a 50% chance of failure
            if (new Random().Next(2) == 0)
            {
                throw new InvalidOperationException("Simulated random failure");
            }
            
            Thread.Sleep(100);
        }

        private static bool ValidateInput(string input)
        {
            // Simple validation that always fails for demonstration
            return !string.IsNullOrEmpty(input) && input.Length > 5;
        }

        private static void SimulateUnreliableOperation()
        {
            // Simulate an operation that fails 70% of the time
            if (new Random().Next(10) < 7)
            {
                throw new TimeoutException("Simulated timeout failure");
            }
            
            Thread.Sleep(50);
        }

        private static void SimulateDataOperation()
        {
            // Always fail for demonstration
            throw new ArgumentException("Invalid data provided to operation");
        }
    }
}
