using System;
using System.Threading;
using Story.Core;

namespace Story.Examples.Scenarios
{
    public static class RotationScenarios
    {
        public static void RunMaxDescendantsRotation()
        {
            Console.WriteLine("--- Max Descendants Rotation Example ---");

            // Configure lower limits for testing
            Storytelling.Configuration.MaxDescendantCount = 10;
            Storytelling.Configuration.MaxLogEntriesCount = 1000;
            Storytelling.Configuration.EnableAutoRotation = true;

            using (var rootStory = Storytelling.StartNew("DescendantTestStory"))
            {
                Storytelling.Info("Starting root story");
                Console.WriteLine($"Root Story: {rootStory.Name}, Instance: {rootStory.InstanceId}, Sequence: {(rootStory.RotationInfo?.Sequence ?? 0)}");

                // Create children until we hit the limit
                for (int i = 0; i < 25; i++)
                {
                    using (var childStory = Storytelling.StartNew($"Child_{i}"))
                    {
                        Storytelling.Info($"Created child {i}");

                        // Check if we're in a new story after rotation
                        var currentStory = Storytelling.Current;
                        if (currentStory.RotationInfo != null)
                        {
                            Console.WriteLine($"\n*** ROTATION OCCURRED ***");
                            Console.WriteLine($"New Story: {currentStory.Name}");
                            Console.WriteLine($"Instance: {currentStory.InstanceId}");
                            Console.WriteLine($"Sequence: {currentStory.RotationInfo.Sequence}");
                            Console.WriteLine($"Origin ID: {currentStory.RotationInfo.OriginInstanceId}");
                            Console.WriteLine($"Rotation Reason: {currentStory.RotationInfo.Reason}");
                            Console.WriteLine($"Root Name: {currentStory.RotationInfo.RootStoryName}\n");
                        }
                    }

                    Thread.Sleep(10); // Small delay for readability
                }

                Storytelling.Info("Completed all child story creation");
            }

            Console.WriteLine("--- Scenario Completed ---\n");
        }

        public static void RunMaxLogsRotation()
        {
            Console.WriteLine("--- Max Logs Rotation Example ---");

            // Configure lower limits for testing
            Storytelling.Configuration.MaxDescendantCount = 1000;
            Storytelling.Configuration.MaxLogEntriesCount = 20;
            Storytelling.Configuration.EnableAutoRotation = true;

            using (var rootStory = Storytelling.StartNew("LogTestStory"))
            {
                Storytelling.Info("Starting root story");
                Console.WriteLine($"Root Story: {rootStory.Name}, Instance: {rootStory.InstanceId}, Sequence: {(rootStory.RotationInfo?.Sequence ?? 0)}");

                // Add logs until we hit the limit
                for (int i = 0; i < 50; i++)
                {
                    Storytelling.Info($"Log entry #{i}");

                    // Check if we're in a new story after rotation
                    var currentStory = Storytelling.Current;
                    if (currentStory.RotationInfo != null && i % 5 == 0)
                    {
                        Console.WriteLine($"\n*** ROTATION OCCURRED (at log #{i}) ***");
                        Console.WriteLine($"New Story: {currentStory.Name}");
                        Console.WriteLine($"Instance: {currentStory.InstanceId}");
                        Console.WriteLine($"Sequence: {currentStory.RotationInfo.Sequence}");
                        Console.WriteLine($"Origin ID: {currentStory.RotationInfo.OriginInstanceId}");
                        Console.WriteLine($"Rotation Reason: {currentStory.RotationInfo.Reason}");
                        Console.WriteLine($"Root Name: {currentStory.RotationInfo.RootStoryName}\n");
                    }

                    Thread.Sleep(5);
                }

                Storytelling.Info("Completed all logging");
            }

            Console.WriteLine("--- Scenario Completed ---\n");
        }

        public static void RunNestedRotation()
        {
            Console.WriteLine("--- Nested Stories with Rotation Example ---");

            // Configure lower limits for testing
            Storytelling.Configuration.MaxDescendantCount = 15;
            Storytelling.Configuration.MaxLogEntriesCount = 1000;
            Storytelling.Configuration.EnableAutoRotation = true;

            using (var rootStory = Storytelling.StartNew("NestedRotationStory"))
            {
                Storytelling.Info("Starting root story with nested children");
                Console.WriteLine($"Root Story: {rootStory.Name}, Sequence: {(rootStory.RotationInfo?.Sequence ?? 0)}");

                // Create nested hierarchies
                for (int i = 0; i < 5; i++)
                {
                    using (var level1 = Storytelling.StartNew($"Level1_{i}"))
                    {
                        Storytelling.Info($"Level 1 - {i}");

                        for (int j = 0; j < 4; j++)
                        {
                            using (var level2 = Storytelling.StartNew($"Level2_{i}_{j}"))
                            {
                                Storytelling.Info($"Level 2 - {i}.{j}");

                                var currentStory = Storytelling.Current;
                                if (currentStory.RotationInfo != null && j == 0)
                                {
                                    Console.WriteLine($"\n*** ROTATION at Level1_{i} ***");
                                    Console.WriteLine($"New Story: {currentStory.Name}, Sequence: {currentStory.RotationInfo.Sequence}\n");
                                }
                            }
                        }
                    }

                    Thread.Sleep(10);
                }

                Storytelling.Info("Completed nested story creation");
            }

            Console.WriteLine("--- Scenario Completed ---\n");
        }

        public static void RunRotationDisabled()
        {
            Console.WriteLine("--- Rotation Disabled Example ---");

            // Disable rotation
            Storytelling.Configuration.MaxDescendantCount = 5;
            Storytelling.Configuration.MaxLogEntriesCount = 10;
            Storytelling.Configuration.EnableAutoRotation = false;

            using (var rootStory = Storytelling.StartNew("DisabledRotationStory"))
            {
                Storytelling.Info("Starting story with rotation disabled");

                // Create more children than the limit
                for (int i = 0; i < 15; i++)
                {
                    using (var child = Storytelling.StartNew($"Child_{i}"))
                    {
                        Storytelling.Info($"Child {i}");
                    }
                }

                var currentStory = Storytelling.Current;
                Console.WriteLine($"Story Name: {currentStory.Name}");
                Console.WriteLine($"Continuation Sequence: {(currentStory.RotationInfo?.Sequence ?? 0)} (should be 0 - no rotation)");

                Storytelling.Info("Completed - no rotation should have occurred");
            }

            // Re-enable rotation for other tests
            Storytelling.Configuration.EnableAutoRotation = true;

            Console.WriteLine("--- Scenario Completed ---\n");
        }
    }
}
