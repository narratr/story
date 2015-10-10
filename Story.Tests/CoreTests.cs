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
            var handlerRules = new Ruleset<IStory, IStoryHandler>()
            {
                Rules = {
                    new PredicateRule(_ => true, _ => new ActionHandler(story => Assert.AreEqual(0, story.Data.Count()), (story, task) => Assert.AreEqual(3, story.Data.Count()))),
                },
            };

            await new Story("testStory", handlerRules).Run(async story =>
            {
                story.Data["bool_value"] = true;
                story.Data["int_value"] = 123;
                story.Data["string_value"] = "test!";
            });
        }
    }
}
