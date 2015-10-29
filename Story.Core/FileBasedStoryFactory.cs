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
    public class FileBasedStoryFactory : IStoryFactory, IDisposable
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
        private IRuleset<IStory, IStoryHandler> ruleset = DefaultRuleset;

        private readonly Func<object[]> rulesetConstructorArgsProvider;

        public FileBasedStoryFactory(string path, Func<object[]> rulesetConstructorArgsProvider = null)
        {
            this.fileWatcher = new FileWatcher(path, OnFileChanged);
            this.rulesetConstructorArgsProvider = rulesetConstructorArgsProvider ?? (() => new object[0]);
        }

        private void OnFileChanged(string fileContent)
        {
            Storytelling.StartNew("FileBasedStoryRulesetProvider", () =>
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
                            var newRuleset = results.CompiledAssembly.CreateInstance(rulesetType.FullName, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, args, null, null) as IRuleset<IStory, IStoryHandler>;
                            this.ruleset = newRuleset;
                            Storytelling.Info("Ruleset updated to {0}", rulesetType.Name);
                        }
                        else
                        {
                            Storytelling.Warn("Missing IRuleset<IStory, IStoryHandler>");
                        }
                    }
                    catch (Exception ex)
                    {
                        Storytelling.Error(ex.ToString());
                    }
                }
                else
                {
                    foreach (var error in results.Errors)
                    {
                        Storytelling.Warn(error.ToString());
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

        public IStory CreateStory(string name)
        {
            return new Story(name, this.ruleset);
        }
    }
}
