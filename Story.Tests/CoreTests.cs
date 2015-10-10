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
        public async Task story_name_is_chained_to_parent()
        {
            var handlerRules = new Ruleset<IStory, IStoryHandler>();

            await new Story("base", handlerRules).Run(async baseStory =>
            {
                await new Story("child", handlerRules).Run(async childStory =>
                {
                    Assert.AreEqual("base/child", childStory.Name);
                });
            });
        }

        [Test]
        public async Task story_name_is_not_chained_to_null_parent()
        {
            var handlerRules = new Ruleset<IStory, IStoryHandler>();

            await new Story("testStory", handlerRules).Run(async story =>
            {
                Assert.AreEqual("testStory", story.Name);
            });
        }

        [Test]
        public async Task story_data_is_observed_during_invocation()
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

            await new Story("testStory", handlerRules).Run(async story =>
            {
                foreach (var kvp in data)
                {
                    story.Data[kvp.Key] = kvp.Value;
                }
            });
        }
    }
}
