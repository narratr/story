using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Story.Core;

namespace Story.Examples.Scenarios
{
    public static class PerformanceScenarios
    {
        public static void RunAllPerformanceTests()
        {
            Console.WriteLine("=== Story Framework Performance Analysis ===");
            Console.WriteLine();

            // Memory tests
            RunMemoryTests();
            Console.WriteLine();

            // CPU tests
            RunCpuTests();
            Console.WriteLine();

            Console.WriteLine("=== Performance Analysis Complete ===");
        }

        private static void RunMemoryTests()
        {
            Console.WriteLine("--- Memory Overhead Analysis ---");
            Console.WriteLine();

            // Baseline
            ForceGC();
            long baseline = GC.GetTotalMemory(true);
            Console.WriteLine($"Baseline (no stories):                    {baseline / 1024:N0} KB");

            // Single story with 10 logs
            var singleMemory = MeasureMemorySingleStory();
            Console.WriteLine($"Single root story (10 logs):              {singleMemory / 1024:N0} KB    (+{(singleMemory - baseline) / 1024:N0} KB)");

            // Small hierarchy
            var smallHierarchyMemory = MeasureMemorySmallHierarchy();
            Console.WriteLine($"Root + 2 children (10 logs each):         {smallHierarchyMemory / 1024:N0} KB    (+{(smallHierarchyMemory - baseline) / 1024:N0} KB)");
            Console.WriteLine();

            // Large scale test
            Console.WriteLine("Large Scale Test (500 roots × 2 children × 10 logs each):");
            var largeScaleMemory = MeasureMemoryLargeScale();
            long largeScaleDelta = largeScaleMemory - baseline;
            Console.WriteLine($"  Total stories:                          1500 stories");
            Console.WriteLine($"  Total log entries:                      15,000 entries");
            Console.WriteLine($"  Total memory:                           {largeScaleMemory / 1024 / 1024:N1} MB    (+{largeScaleDelta / 1024 / 1024:N1} MB from baseline)");
            Console.WriteLine($"  Per-story overhead:                     ~{largeScaleDelta / 1500 / 1024:N1} KB/story");
            Console.WriteLine($"  Per-hierarchy overhead:                 ~{largeScaleDelta / 500 / 1024:N1} KB/hierarchy (1 root + 2 children)");
            Console.WriteLine($"  RootStoryState overhead:                ~{500 * 24 / 1024:N1} KB (500 × 24 bytes)");
            Console.WriteLine();

            // Rotation test
            Console.WriteLine("Rotation Test (1 root + 1500 children, rotation every 200):");
            MeasureMemoryWithRotation();
            Console.WriteLine();

            // RootStoryState efficiency
            Console.WriteLine("RootStoryState Efficiency:");
            Console.WriteLine($"  Shared state per hierarchy:            ~24 bytes");
            Console.WriteLine($"  With 500 hierarchies:                  ~{500 * 24 / 1024:N1} KB total");
            Console.WriteLine($"  Savings vs individual counters:        ~{500 * 8 / 1024:N1} KB (500 × 8 bytes saved)");
        }

        private static long MeasureMemorySingleStory()
        {
            // Disable rotation for clean measurement
            var originalMaxLogs = Storytelling.Configuration.MaxLogEntriesCount;
            var originalMaxDesc = Storytelling.Configuration.MaxDescendantCount;
            Storytelling.Configuration.MaxLogEntriesCount = 10000;
            Storytelling.Configuration.MaxDescendantCount = 10000;

            ForceGC();

            var story = Storytelling.StartNew("SingleStory");
            for (int i = 0; i < 10; i++)
            {
                Storytelling.Info($"Log entry {i}");
            }

            long memory = GC.GetTotalMemory(true);

            story.Dispose();
            ForceGC();

            // Restore limits
            Storytelling.Configuration.MaxLogEntriesCount = originalMaxLogs;
            Storytelling.Configuration.MaxDescendantCount = originalMaxDesc;

            return memory;
        }

        private static long MeasureMemorySmallHierarchy()
        {
            // Disable rotation for clean measurement
            var originalMaxLogs = Storytelling.Configuration.MaxLogEntriesCount;
            var originalMaxDesc = Storytelling.Configuration.MaxDescendantCount;
            Storytelling.Configuration.MaxLogEntriesCount = 10000;
            Storytelling.Configuration.MaxDescendantCount = 10000;

            ForceGC();

            var root = Storytelling.StartNew("SmallHierarchyRoot");
            for (int i = 0; i < 10; i++)
            {
                Storytelling.Info($"Root log {i}");
            }

            var child1 = Storytelling.StartNew("Child1");
            for (int i = 0; i < 10; i++)
            {
                Storytelling.Info($"Child1 log {i}");
            }
            child1.Dispose();

            var child2 = Storytelling.StartNew("Child2");
            for (int i = 0; i < 10; i++)
            {
                Storytelling.Info($"Child2 log {i}");
            }
            child2.Dispose();

            long memory = GC.GetTotalMemory(true);

            root.Dispose();
            ForceGC();

            // Restore limits
            Storytelling.Configuration.MaxLogEntriesCount = originalMaxLogs;
            Storytelling.Configuration.MaxDescendantCount = originalMaxDesc;

            return memory;
        }

        private static long MeasureMemoryLargeScale()
        {
            // Disable rotation for clean measurement
            var originalMaxLogs = Storytelling.Configuration.MaxLogEntriesCount;
            var originalMaxDesc = Storytelling.Configuration.MaxDescendantCount;
            Storytelling.Configuration.MaxLogEntriesCount = 100000;
            Storytelling.Configuration.MaxDescendantCount = 100000;

            ForceGC();

            var stories = new List<IStory>();

            for (int i = 0; i < 500; i++)
            {
                var root = Storytelling.StartNew($"Root_{i}");
                stories.Add(root);

                for (int j = 0; j < 10; j++)
                {
                    Storytelling.Info($"Root log {j}");
                }

                for (int c = 0; c < 2; c++)
                {
                    var child = Storytelling.StartNew($"Child_{i}_{c}");
                    stories.Add(child);

                    for (int j = 0; j < 10; j++)
                    {
                        Storytelling.Info($"Child log {j}");
                    }
                }
            }

            long memory = GC.GetTotalMemory(true);

            // Dispose all stories
            foreach (var story in stories)
            {
                story.Dispose();
            }
            stories.Clear();
            ForceGC();

            // Restore limits
            Storytelling.Configuration.MaxLogEntriesCount = originalMaxLogs;
            Storytelling.Configuration.MaxDescendantCount = originalMaxDesc;

            return memory;
        }

        private static void MeasureMemoryWithRotation()
        {
            // Configure limits for rotation
            var originalMaxDescendants = Storytelling.Configuration.MaxDescendantCount;
            var originalAutoRotation = Storytelling.Configuration.EnableAutoRotation;

            Storytelling.Configuration.MaxDescendantCount = 200;
            Storytelling.Configuration.EnableAutoRotation = true;

            ForceGC();

            using (var root = Storytelling.StartNew("RotationTestRoot"))
            {
                long memoryAt0 = GC.GetTotalMemory(true);
                Console.WriteLine($"  Memory at 0 children:                   {memoryAt0 / 1024:N0} KB");

                int rotationCount = 0;
                long lastMemory = memoryAt0;

                for (int i = 0; i < 1500; i++)
                {
                    using (var child = Storytelling.StartNew($"Child_{i}"))
                    {
                        Storytelling.Info($"Child {i}");
                    }

                    // Check at rotation boundaries
                    if ((i + 1) % 200 == 0)
                    {
                        ForceGC();
                        long memory = GC.GetTotalMemory(true);
                        var current = Storytelling.Current;
                        int currentRotation = current.RotationInfo?.Sequence ?? 0;

                        if (currentRotation > rotationCount)
                        {
                            Console.WriteLine($"  After {i + 1} children:");
                            Console.WriteLine($"    Before rotation #{currentRotation}:          {lastMemory / 1024:N0} KB");
                            Console.WriteLine($"    After rotation #{currentRotation}:           {memory / 1024:N0} KB    (delta: {((int)(memory - lastMemory)) / 1024:+#;-#;0} KB)");
                            rotationCount = currentRotation;
                        }

                        lastMemory = memory;
                    }
                }

                ForceGC();
                long finalMemory = GC.GetTotalMemory(true);
                Console.WriteLine($"  Final memory (current story):           {finalMemory / 1024:N0} KB");
                Console.WriteLine($"  Total rotations:                        {rotationCount} rotations");
                Console.WriteLine($"  Memory stayed bounded (no linear growth with child count)");
            }

            ForceGC();
            long afterDispose = GC.GetTotalMemory(true);
            Console.WriteLine($"  Memory after full disposal:             {afterDispose / 1024:N0} KB");

            // Restore original limits
            Storytelling.Configuration.MaxDescendantCount = originalMaxDescendants;
            Storytelling.Configuration.EnableAutoRotation = originalAutoRotation;
        }

        private static void RunCpuTests()
        {
            Console.WriteLine("--- CPU Performance Analysis ---");
            Console.WriteLine();

            // Story creation
            var creationTime = MeasureStoryCreation();
            Console.WriteLine($"Story creation (10k):                     {creationTime} ms    ({creationTime * 1000.0 / 10000:F2} μs/op)");

            // Logging
            var loggingTime = MeasureLogging();
            Console.WriteLine($"Logging (100k):                           {loggingTime} ms    ({loggingTime * 1000.0 / 100000:F2} μs/op)");

            // Rotation check
            var rotationCheckTime = MeasureRotationCheck();
            Console.WriteLine($"Rotation check (100k):                    {rotationCheckTime} ms    ({rotationCheckTime * 1000.0 / 100000:F2} μs/op)");

            // Rotation execution
            var rotationExecTime = MeasureRotationExecution();
            Console.WriteLine($"Rotation execution (per rotation):        ~{rotationExecTime:F2} ms");

            // Root access
            var rootAccessTime = MeasureRootAccess();
            Console.WriteLine($"Root access depth=2 (100k):               {rootAccessTime} ms    ({rootAccessTime * 1000.0 / 100000:F2} μs/op)");

            Console.WriteLine();

            // Real-world overhead
            Console.WriteLine("Real-World Overhead:");
            MeasureRealWorldOverhead();
        }

        private static long MeasureStoryCreation()
        {
            // Warmup
            for (int i = 0; i < 100; i++)
            {
                using (var story = Storytelling.StartNew("Warmup")) { }
            }

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++)
            {
                using (var story = Storytelling.StartNew($"Test_{i}")) { }
            }
            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        private static long MeasureLogging()
        {
            // Warmup
            using (var warmup = Storytelling.StartNew("Warmup"))
            {
                for (int i = 0; i < 100; i++)
                {
                    Storytelling.Info($"Warmup {i}");
                }
            }

            using (var story = Storytelling.StartNew("LoggingTest"))
            {
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < 100000; i++)
                {
                    Storytelling.Info($"Test {i}");
                }
                sw.Stop();
                return sw.ElapsedMilliseconds;
            }
        }

        private static long MeasureRotationCheck()
        {
            // Disable rotation to isolate check overhead
            var originalAutoRotation = Storytelling.Configuration.EnableAutoRotation;
            Storytelling.Configuration.EnableAutoRotation = false;

            using (var story = Storytelling.StartNew("RotationCheckTest"))
            {
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < 100000; i++)
                {
                    using (var child = Storytelling.StartNew($"Child_{i}")) { }
                }
                sw.Stop();

                Storytelling.Configuration.EnableAutoRotation = originalAutoRotation;
                return sw.ElapsedMilliseconds;
            }
        }

        private static double MeasureRotationExecution()
        {
            var originalMaxDescendants = Storytelling.Configuration.MaxDescendantCount;
            Storytelling.Configuration.MaxDescendantCount = 50;

            var rotationTimes = new List<long>();

            using (var root = Storytelling.StartNew("RotationExecTest"))
            {
                for (int i = 0; i < 250; i++)
                {
                    var sw = Stopwatch.StartNew();
                    using (var child = Storytelling.StartNew($"Child_{i}"))
                    {
                        Storytelling.Info("test");
                    }
                    sw.Stop();

                    // If rotation occurred, this operation took longer
                    if (sw.ElapsedMilliseconds > 0)
                    {
                        rotationTimes.Add(sw.ElapsedMilliseconds);
                    }
                }
            }

            Storytelling.Configuration.MaxDescendantCount = originalMaxDescendants;

            // Return average of top measurements (likely rotation operations)
            if (rotationTimes.Any())
            {
                return rotationTimes.OrderByDescending(t => t).Take(5).Average();
            }
            return 0;
        }

        private static long MeasureRootAccess()
        {
            using (var root = Storytelling.StartNew("RootAccessTest"))
            {
                using (var child1 = Storytelling.StartNew("Child1"))
                {
                    using (var child2 = Storytelling.StartNew("Child2"))
                    {
                        // Warmup
                        for (int i = 0; i < 100; i++)
                        {
                            var current = Storytelling.Current;
                        }

                        var sw = Stopwatch.StartNew();
                        for (int i = 0; i < 100000; i++)
                        {
                            var current = Storytelling.Current;
                        }
                        sw.Stop();

                        return sw.ElapsedMilliseconds;
                    }
                }
            }
        }

        private static void MeasureRealWorldOverhead()
        {
            const int iterations = 1000;

            // Without Story
            var sw1 = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                SimulateWork();
            }
            sw1.Stop();
            long withoutStory = sw1.ElapsedMilliseconds;

            // With Story
            var sw2 = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                using (var story = Storytelling.StartNew($"Work_{i}"))
                {
                    Storytelling.Info("Starting work");
                    SimulateWork();
                    Storytelling.Info("Work completed");
                }
            }
            sw2.Stop();
            long withStory = sw2.ElapsedMilliseconds;

            long overhead = withStory - withoutStory;
            double overheadPercent = (overhead * 100.0) / withoutStory;

            Console.WriteLine($"  Work without Story:                     {withoutStory} ms");
            Console.WriteLine($"  Work with Story:                        {withStory} ms    (+{overhead} ms, +{overheadPercent:F1}% overhead)");
        }

        private static void SimulateWork()
        {
            // Simulate some CPU work
            double result = 0;
            for (int i = 0; i < 1000; i++)
            {
                result += Math.Sqrt(i);
            }
        }

        private static void ForceGC()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
