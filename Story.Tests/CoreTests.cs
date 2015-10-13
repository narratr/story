namespace Story.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using NUnit.Framework;

    using Story.Core;
    using Story.Core.Rules;
    using Story.Core.Handlers;

    [TestFixture]
    public class CoreTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void story_name_cannot_be_null()
        {
            var story = new Story(null, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void story_handler_rules_cannot_be_null()
        {
            var story = new Story("story", null);
        }

        [Test]
        public void story_detaches_correctly()
        {
            var handlerRules = new Ruleset<IStory, IStoryHandler>();

            var story1 = new Story("story1", handlerRules);
            Assert.AreEqual("story1", story1.Name);
            story1.Detach();

            var story2 = new Story("story2", handlerRules);
            Assert.AreEqual("story2", story2.Name);
            story2.Detach();
        }

        [Test]
        public void story_name_is_chained_to_parent()
        {
            var handlerRules = new Ruleset<IStory, IStoryHandler>();

            new Story("base", handlerRules).Run(baseStory =>
            {
                new Story("child", handlerRules).Run(childStory =>
                {
                    Assert.AreEqual("base/child", childStory.Name);
                });
            });
        }

        [Test]
        public void story_name_is_not_chained_to_null_parent()
        {
            var handlerRules = new Ruleset<IStory, IStoryHandler>();

            new Story("testStory", handlerRules).Run(story =>
            {
                Assert.AreEqual("testStory", story.Name);
            });
        }

        [Test]
        public void story_data_is_observed_during_invocation()
        {
            var data = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("bool_value", true),
                new KeyValuePair<string, object>("int_value", 123),
                new KeyValuePair<string, object>("string_value", "test!"),
            };

            var handlerRules = new Ruleset<IStory, IStoryHandler>()
            {
                Rules = {
                    new PredicateRule(
                        _ => true,                                                              // always run for story
                        _ => new ActionHandler(
                            (story) => Assert.AreEqual(0, story.Data.Count()),                  // make sure OnStart() is invoked with zero data items.
                            (story, task) => Assert.IsTrue(data.SequenceEqual(story.Data)))     // make sure OnStop() is invoked with 3 data items.
                    ),
                },
            };

            new Story("testStory", handlerRules).Run(story =>
            {
                foreach (var kvp in data)
                {
                    story.Data[kvp.Key] = kvp.Value;
                }
            });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage ="oh oh")]
        public void story_exception_thrown_is_propagated()
        {
            var handlerRules = new Ruleset<IStory, IStoryHandler>();

            new Story("testStory", handlerRules).Run(story =>
            {
                throw new InvalidOperationException("oh oh");
            });
        }

        [Test]
        public void story_handler_is_invoked_when_rule_condition_is_met()
        {
            var invokedBefore = false;
            var invokedAfter = false;

            var handlerRules = new Ruleset<IStory, IStoryHandler>()
            {
                Rules =  {
                    new PredicateRule(story =>
                    {
                        var userId = (string)story.Data["userId"];
                        return userId != null && userId == "user13";
                    },
                    _ => new ActionHandler(s => invokedBefore = true, (s, task) => invokedAfter = true)),
                }
            };

            var testStory = new Story("testStory", handlerRules);
            testStory.Data["userId"] = "user13";

            testStory.Run(story => {});

            Assert.IsTrue(invokedBefore);
            Assert.IsTrue(invokedAfter);
        }

        [Test]
        public void story_handler_is_not_invoked_when_rule_condition_is_not_met()
        {
            var invokedBefore = false;
            var invokedAfter = false;

            var handlerRules = new Ruleset<IStory, IStoryHandler>()
            {
                Rules =  {
                    new PredicateRule(story =>
                    {
                        var userId = (string)story.Data["userId"];
                        return userId != null && userId == "user13";
                    },
                    _ => new ActionHandler(s => invokedBefore = true, (s, task) => invokedAfter = true)),
                }
            };

            var testStory = new Story("testStory", handlerRules);
            testStory.Data["userId"] = "user21";

            testStory.Run(story => { });

            Assert.IsFalse(invokedBefore);
            Assert.IsFalse(invokedAfter);
        }

        [Test]
        public void story_log_is_observed_during_invocation()
        {
            var log = new List<KeyValuePair<LogSeverity, string>>
            {
                new KeyValuePair<LogSeverity, string>( LogSeverity.Info, "test_info"),
                new KeyValuePair<LogSeverity, string>( LogSeverity.Warning, "test_warning"),
                new KeyValuePair<LogSeverity, string>( LogSeverity.Error, "test_error"),
            };

            var handlerRules = new Ruleset<IStory, IStoryHandler>()
            {
                Rules = {
                    new PredicateRule(
                        _ => true,                                                              // always run for story
                        _ => new ActionHandler(
                            (story) => Assert.AreEqual(0, story.Log.Count()),                   // make sure OnStart() is invoked with zero log items.
                            (story, task) => Assert.IsTrue(story.Log.All(
                                entry => log.Exists(
                                    l => l.Key == entry.Severity &&
                                    l.Value == entry.Text))))
                    ),
                },
            };

            new Story("testStory", handlerRules).Run(story =>
            {
                foreach (var kvp in log)
                {
                    story.Log.Add(kvp.Key, kvp.Value);
                }
            });

        }
    }
}
