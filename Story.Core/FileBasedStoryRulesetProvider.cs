using Microsoft.CSharp;
using Story.Core.Handlers;
using Story.Core.Rules;
using Story.Core.Utils;
using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;

namespace Story.Core
{
    public class FileBasedStoryRulesetProvider : IStoryRulesetProvider, IDisposable
    {
        /*
            Sample file:

            using Story.Core;
            using Story.Core.Handlers;
            using Story.Core.Rules;
            using System;
            using System.Linq;

            public class StoryRuleset : Ruleset<IStory, IStoryHandler>
            {
                public StoryRuleset(AnalyticsService analyticsService, IStoryRulesetProvider storyRulesetProvider)
                {
                    Rules.Add(new PredicateRule(story => true, _ => new TraceHandler("trace", StoryFormatters.BasicStoryFormatter)));
                }
            }
        */

        private readonly static IRuleset<IStory, IStoryHandler> DefaultRuleset = new Ruleset<IStory, IStoryHandler>()
        {
            Rules =
            {
                new PredicateRule(_ => true, _ => new TraceHandler("trace", StoryFormatters.BasicStoryFormatter))
            }
        };

        private FileWatcher fileWatcher;
        private IRuleset<IStory, IStoryHandler> ruleset;

        private readonly Func<object[]> rulesetConstructorArgsProvider;

        public FileBasedStoryRulesetProvider(string path, Func<object[]> rulesetConstructorArgsProvider = null)
        {
            this.fileWatcher = new FileWatcher(path, OnFileChanged);
            this.rulesetConstructorArgsProvider = rulesetConstructorArgsProvider ?? (() => new object[0]);
        }

        private void OnFileChanged(string fileContent)
        {
            new Story("FileBasedStoryRulesetProvider", DefaultRuleset).Run(story =>
            {
                // Create a new instance of the C# compiler
                var compiler = new CSharpCodeProvider();

                // Create some parameters for the compiler
                var parms = new CompilerParameters()
                {
                    GenerateExecutable = false,
                    GenerateInMemory = true,
                    TreatWarningsAsErrors = false
                };

                // Load assemblies from current domain
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        parms.ReferencedAssemblies.Add(assembly.Location);
                        foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
                        {
                            parms.ReferencedAssemblies.Add(Assembly.Load(assemblyName).Location);
                        }
                    }
                    catch
                    {
                    }
                }

                // Try to compile the string into an assembly
                var results = compiler.CompileAssemblyFromSource(parms, fileContent);

                // Create ruleset
                if (results.Errors.Count == 0)
                {
                    try
                    {
                        var rulesetType = results.CompiledAssembly.DefinedTypes.FirstOrDefault(definedType => definedType.GetInterfaces().Any(i => i == typeof(IRuleset<IStory, IStoryHandler>)));
                        if (rulesetType != null)
                        {
                            var args = this.rulesetConstructorArgsProvider();
                            var ruleset = results.CompiledAssembly.CreateInstance(rulesetType.FullName, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, args, null, null) as IRuleset<IStory, IStoryHandler>;
                            this.ruleset = ruleset;
                            story.Log.Info("Ruleset updated to {0}", rulesetType.Name);
                        }
                        else
                        {
                            story.Log.Warn("Missing IRuleset<IStory, IStoryHandler>");
                        }
                    }
                    catch (Exception ex)
                    {
                        story.Log.Error(ex.ToString());
                    }
                }
                else
                {
                    foreach (var error in results.Errors)
                    {
                        story.Log.Warn(error.ToString());
                    }
                }
            });
        }

        public IRuleset<IStory, IStoryHandler> GetRuleset()
        {
            return this.ruleset;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.fileWatcher != null)
                {
                    this.fileWatcher.Dispose();
                    this.fileWatcher = null;
                }
            }
        }
    }
}
