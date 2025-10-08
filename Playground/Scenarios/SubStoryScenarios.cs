using System;
using System.Threading;
using Story.Core;

namespace Story.Examples.Scenarios
{
    public static class SubStoryScenarios
    {
        public static void RunSimpleSubStoryExample()
        {
            Console.WriteLine("--- Simple Sub-Story Example ---");
            
            using (var mainStory = Storytelling.StartNew("MainStory"))
            {
                Storytelling.Info("Starting main story");
                Storytelling.Data["mainStoryData"] = "Main story data";
                
                // Create a sub-story
                using (var subStory = Storytelling.StartNew("SubStory"))
                {
                    Storytelling.Info("Executing sub-story");
                    Storytelling.Data["subStoryData"] = "Sub-story data";
                    Thread.Sleep(100);
                    Storytelling.Info("Sub-story completed");
                }
                
                Storytelling.Info("Main story completed");
            }
        }

        public static void RunNestedSubStoryExample()
        {
            Console.WriteLine("--- Nested Sub-Story Example ---");
            
            using (var level1Story = Storytelling.StartNew("Level1Story"))
            {
                Storytelling.Info("Starting level 1 story");
                
                using (var level2Story = Storytelling.StartNew("Level2Story"))
                {
                    Storytelling.Info("Starting level 2 story");
                    
                    using (var level3Story = Storytelling.StartNew("Level3Story"))
                    {
                        Storytelling.Info("Starting level 3 story (deepest level)");
                        Thread.Sleep(50);
                        Storytelling.Info("Level 3 story completed");
                    }
                    
                    Thread.Sleep(50);
                    Storytelling.Info("Level 2 story completed");
                }
                
                Thread.Sleep(50);
                Storytelling.Info("Level 1 story completed");
            }
        }

        public static void RunSequentialSubStoriesExample()
        {
            Console.WriteLine("--- Sequential Sub-Stories Example ---");
            
            using (var mainStory = Storytelling.StartNew("SequentialMainStory"))
            {
                Storytelling.Info("Starting main story with sequential sub-stories");
                
                // Run sub-stories sequentially
                using (var subStory1 = Storytelling.StartNew("SequentialSubStory1"))
                {
                    Storytelling.Info("Starting SequentialSubStory1");
                    Thread.Sleep(100);
                    Storytelling.Info("Completed SequentialSubStory1");
                }
                
                using (var subStory2 = Storytelling.StartNew("SequentialSubStory2"))
                {
                    Storytelling.Info("Starting SequentialSubStory2");
                    Thread.Sleep(150);
                    Storytelling.Info("Completed SequentialSubStory2");
                }
                
                using (var subStory3 = Storytelling.StartNew("SequentialSubStory3"))
                {
                    Storytelling.Info("Starting SequentialSubStory3");
                    Thread.Sleep(75);
                    Storytelling.Info("Completed SequentialSubStory3");
                }
                
                Storytelling.Info("All sequential sub-stories completed");
            }
        }

        public static void RunComplexHierarchyExample()
        {
            Console.WriteLine("--- Complex Hierarchy Example ---");
            
            using (var rootStory = Storytelling.StartNew("RootStory"))
            {
                Storytelling.Info("Starting root story");
                
                // Branch 1
                using (var branch1 = Storytelling.StartNew("Branch1"))
                {
                    Storytelling.Info("Starting branch 1");
                    
                    using (var leaf1a = Storytelling.StartNew("Leaf1A"))
                    {
                        Storytelling.Info("Processing leaf 1A");
                        Thread.Sleep(50);
                    }
                    
                    using (var leaf1b = Storytelling.StartNew("Leaf1B"))
                    {
                        Storytelling.Info("Processing leaf 1B");
                        Thread.Sleep(75);
                    }
                    
                    Storytelling.Info("Branch 1 completed");
                }
                
                // Branch 2
                using (var branch2 = Storytelling.StartNew("Branch2"))
                {
                    Storytelling.Info("Starting branch 2");
                    
                    using (var leaf2a = Storytelling.StartNew("Leaf2A"))
                    {
                        Storytelling.Info("Processing leaf 2A");
                        Thread.Sleep(60);
                    }
                    
                    using (var leaf2b = Storytelling.StartNew("Leaf2B"))
                    {
                        Storytelling.Info("Processing leaf 2B");
                        Thread.Sleep(40);
                    }
                    
                    Storytelling.Info("Branch 2 completed");
                }
                
                Storytelling.Info("Root story completed");
            }
        }

        public static void RunDataFlowExample()
        {
            Console.WriteLine("--- Data Flow Example ---");
            
            using (var parentStory = Storytelling.StartNew("ParentStory"))
            {
                Storytelling.Info("Starting parent story");
                Storytelling.Data["parentData"] = "Data from parent";
                Storytelling.Data["sharedCounter"] = 0;
                
                // Child 1
                using (var child1 = Storytelling.StartNew("Child1"))
                {
                    Storytelling.Info("Starting child 1");
                    Storytelling.Data["child1Data"] = "Data from child 1";
                    
                    // Access parent data
                    var parentData = Storytelling.Data["parentData"];
                    Storytelling.Info($"Child 1 accessed parent data: {parentData}");
                    
                    // Update shared data
                    var counter = Storytelling.Data["sharedCounter"] != null ? (int)Storytelling.Data["sharedCounter"] : 0;
                    Storytelling.Data["sharedCounter"] = counter + 1;
                    Storytelling.Info($"Child 1 updated shared counter to: {Storytelling.Data["sharedCounter"]}");
                }
                
                // Child 2
                using (var child2 = Storytelling.StartNew("Child2"))
                {
                    Storytelling.Info("Starting child 2");
                    Storytelling.Data["child2Data"] = "Data from child 2";
                    
                    // Access parent data
                    var parentData = Storytelling.Data["parentData"];
                    Storytelling.Info($"Child 2 accessed parent data: {parentData}");
                    
                    // Update shared data
                    var counter = Storytelling.Data["sharedCounter"] != null ? (int)Storytelling.Data["sharedCounter"] : 0;
                    Storytelling.Data["sharedCounter"] = counter + 1;
                    Storytelling.Info($"Child 2 updated shared counter to: {Storytelling.Data["sharedCounter"]}");
                }
                
                Storytelling.Info($"Parent story completed. Final shared counter: {Storytelling.Data["sharedCounter"]}");
            }
        }
    }
}
