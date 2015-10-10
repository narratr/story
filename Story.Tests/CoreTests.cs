namespace Story.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using NUnit.Framework;

    using Story.Core;

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
            var ruleset = new Ruleset<IStory, IStoryHandler>();

            var story1 = new Story("story1", ruleset);
            Assert.AreEqual("story1", story1.Name);
            story1.Detach();

            var story2 = new Story("story2", ruleset);
            Assert.AreEqual("story2", story2.Name);
            story2.Detach();
        }

        [Test]
        public async Task story_name_is_chained_to_parent()
        {
            var ruleset = new Ruleset<IStory, IStoryHandler>();

            await new Story("base", ruleset).Run(async baseStory =>
            {
                await new Story("child", ruleset).Run(async childStory =>
                {
                    Assert.AreEqual("base/child", childStory.Name);
                });
            });
        }

        [Test]
        public async Task story_name_is_not_chained_to_null_parent()
        {
            var ruleset = new Ruleset<IStory, IStoryHandler>();

            await new Story("testStory", ruleset).Run(async story =>
            {
                Assert.AreEqual("testStory", story.Name);
            });
        }

    }
}
