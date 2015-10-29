namespace Story.Tests
{
    using NUnit.Framework;
    using Story.Core;
    using Story.Core.Handlers;
    using Story.Core.Rules;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestFixture]
    public class CoreTests
    {
        [SetUp]
        public void Setup()
        {
            Storytelling.Factory = new BasicStoryFactory(new Ruleset<IStory, IStoryHandler>());
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

            Storytelling.StartNew("base", () =>
            {
                Storytelling.StartNew("child", () =>
                {
                    Assert.AreEqual("base/child", Storytelling.Current.Name);
                });
            });
        }

        [Test]
        public void story_name_is_not_chained_to_null_parent()
        {
            var handlerRules = new Ruleset<IStory, IStoryHandler>();

            Storytelling.StartNew("testStory", () =>
            {
                Assert.AreEqual("testStory", Storytelling.Current.Name);
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
                        _ => true,                                                        // always run for story
                        _ => new ActionHandler(
                            "data_verification_handler",
                            (story) => Assert.AreEqual(0, story.Data.Count()),            // make sure OnStart() is invoked with zero data items.
                            (story) => Assert.IsTrue(data.SequenceEqual(story.Data)))     // make sure OnStop() is invoked with 3 data items.
                    ),
                },
            };

            Storytelling.StartNew("testStory", () =>
            {
                foreach (var kvp in data)
                {
                    Storytelling.Data[kvp.Key] = kvp.Value;
                }
            });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "oh oh")]
        public void story_exception_thrown_is_propagated()
        {
            var handlerRules = new Ruleset<IStory, IStoryHandler>();

            Storytelling.StartNew("testStory", () =>
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
                    _ => new ActionHandler("invocation_handler", s => invokedBefore = true, s => invokedAfter = true)),
                }
            };

            Storytelling.Factory = new BasicStoryFactory(handlerRules);

            Storytelling.StartNew("testStory", () =>
            {
                Storytelling.Data["userId"] = "user13";

                Assert.IsTrue(invokedBefore);
                Assert.IsTrue(invokedAfter);
            });
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
                    _ => new ActionHandler("invocation_handler", s => invokedBefore = true, s => invokedAfter = true)),
                }
            };

            Storytelling.Factory = new BasicStoryFactory(handlerRules);

            Storytelling.StartNew("testStory", () =>
            {
                Storytelling.Data["userId"] = "user21";

                Assert.IsFalse(invokedBefore);
                Assert.IsFalse(invokedAfter);
            });
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
                            "data_verification_handler",
                            (story) => Assert.AreEqual(0, story.Log.Count()),                   // make sure OnStart() is invoked with zero log items.
                            (story) => Assert.IsTrue(story.Log.All(
                                entry => log.Exists(
                                    l => l.Key == entry.Severity &&
                                    l.Value == entry.Text))))
                    ),
                },
            };

            Storytelling.Factory = new BasicStoryFactory(handlerRules);

            Storytelling.StartNew("testStory", () =>
            {
                foreach (var kvp in log)
                {
                    Storytelling.Current.Log.Add(kvp.Key, kvp.Value);
                }
            });
        }
    }
}
