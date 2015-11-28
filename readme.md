# Story framework

Don't just write logs, understand what's going on.

## Sample

```csharp
    Storytelling.Factory.StartNew("MyAction", () =>
    {
        var i = 0;
        i++;
        Storytelling.Info("added 1 to i"); // log
        Storytelling.Data["i"] = i;         // data
    });

    // Output (to the .net trace):
    // Story MyAction (ebdfcef5-f772-4dfd-8f0f-580eb79d1bb1) on rule Trace
    // i - 1

    // +7.0024 ms Info added 1 to i
    // +7.0024 ms Debug Added key 'i' to data.

    // - Story "MyAction" took 7 ms
```

## What

A rule-based, contextual logging and telemetry framework for .NET.

## Why

In the traditional world logging it's usually all about just dumping pieces of information along the way (of the code).

The problem with that is later making sense of all that information.

We believe a new approach needs to be taken here, where these logs, telemetry and any important piece of information are stored in a context and decisions on what to do with this information can be done on the fly.

## Story framework solution

a story is a container of information, you create and start it at the begining of an operation/work and stop it at the end.

During that time you can add logs, any bits of information (data) to it and even new (sub)stories.

At the end (when you stop a story), a set of rules (you define) will run on it and decide what to do with it, for example:

* If the story is important (your definition of it), print it to the trace (console/nlog/...).
* If the story has an error, send an email with it.
* If the average of the last 20 list operations took longer than 200 ms, alert with an SMS.
* If the story has a "new user joined" operation, send event to mixpanel.

etc...

### Cool

These rules can be updated on the fly which opens up a scenario, for example, where you have a user complaining so you can update the rules to send all stories with that user to a specific storage container so you can easily help him.

### Notes

* The story is stored in the `CallContext/HttpContext` for easy usage so you don't need to pass the `story` object around.
* The story has some default properties it stores like: start date, elapsed time and instance id.
* The rules will also run at the start of a story to allow scenarios like increment/decrement of performance counters.
* The work on the story framework is an iopen source and a work in progress with the hopes of getting **your** contributions or alternatively your complaints/issues/feature requests.

## Usage

Install via NuGet - https://nuget.org/packages/Story

### Initialization

By default all stories are outputed to the trace, but you can (and should) set your own ruleset

```cshaprp
    // Create the ruleset
    var ruleset = new Ruleset<IStory, IStoryHandler>();

    // Add a new predicate rule, for any story run the console handler which prints the story to the console
    var consoleHandler = new ConsoleHandler("PrintToConsole", StoryFormatters.GetBasicStoryFormatter(LogSeverity.Debug));
    ruleset.Rules.Add(
        new PredicateRule( 
            story => true,
            story => consoleHandler));

    // Set a new basic factory that uses the ruleset as the default story factory
    Storytelling.Factory = new BasicStoryFactory(ruleset);
````

### Initialization with ruleset that can change on the fly

The way this works is by creating a story factory (`FileBasedStoryFactory`) that listens for changes in a single .cs file with the ruleset (`StoryRuleset`).
The story factory will build the .cs file and use the ruleset when initialized and whenever the file updates.

```csharp
    Storytelling.Factory =
        new FileBasedStoryFactory(
            "Path\\To\\RulesetFile.cs");
```

It's also possible to pass arguments to the ruleset constructor

```csharp
    var myService = new MyService();

    Storytelling.Factory =
        new FileBasedStoryFactory(
            "Path\\To\\RulesetFile.cs",
            () => new object[]
            {
                myService
            });
```

And the RulesetFile.cs

```csharp
    using Story.Core;
    using Story.Core.Handlers;
    using Story.Core.Rules;
    using System;
    using System.Linq;

    public class StoryRuleset : Ruleset<IStory, IStoryHandler>
    {
        public StoryRuleset(AnalyticsService analyticsService, IStoryRulesetProvider storyRulesetProvider)
        {
            Rules.Add(
                new PredicateRule(
                    story => story.IsRoot() && story.GetDataValue("userId") as string == "123456",
                    story => StoryHandlers.DefaultTraceHandler));
        }
    }

```

### Run operation within a story context

```csharp
    Storytelling.Factory.StartNew("MyAction", () =>
    {
        var i = 0;
        i++;
        Storytelling.Info("added 1 to i"); // log
        Storytelling.Data["i"] = i;        // data
    });
```

Also possible to run an async operation

```csharp
    await Storytelling.Factory.StartNewAsync("some name", async () =>
    {
        var i = 0;
        i++;

        await RunAsync();

        Storytelling.Debug("added 1 to i"); // log
        Storytelling.Data["i"] = i;         // data
    });
```

The story is in the `CallContext/HttpContext` so no need to pass the story around to called methods

```csharp
    private static async Task RunAsync()
    {
        var result = await GetResult();
        Storytelling.Data["result"] = result;
    }
```

> Note: the following code will throw an exception if there is no story in the context, it is our recommendation to always run within a story context if you want to use it.
> But if you want to not throw this exception (and simply ignore any call to log or add data to a story if it doesn't exist in the context), use `Storytelling.IgnoreEmptyStory`

```csharp
    Storytelling.IgnoreEmptyStory.Data["result"] = result;
```

### Logging

```csharp
                Storytelling.Debug("Debug - {0}", 1);

                Storytelling.Info("Information");

                Storytelling.Warn("Warning");

                Storytelling.Error("Error");

                Storytelling.Critical("Critical");
```

### Adding relevant information/data to the story

```csharp
    Storytelling.Data["user"] = user;
```

> Note: Try to add only (json) serializable objects to the `Data`, otherwise some scenarios will not work (like storing the story to azure table storage and anything similar that requires serialization)

### Owin middleware that encapsulates a request with a story

Story framework and web applications are  made for each other, you can encapsulate each request in a story that give you all the information you need about it.

```csharp
    /// <summary>
    /// Story middleware
    /// </summary>
    public class StoryMiddleware : OwinMiddleware
    {
        public StoryMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            var request = context.Request;
            return Storytelling.StartNewAsync("Request", async () =>
            {
                try
                {
                    Storytelling.Data["RequestUrl"] = request.Uri.ToString();
                    Storytelling.Data["RequestMethod"] = request.Method;
                    Storytelling.Data["UserIp"] = request.RemoteIpAddress;
                    Storytelling.Data["UserAgent"] = request.Headers.Get("User-Agent");
                    Storytelling.Data["Referer"] = request.Headers.Get("Referer");

                    await Next.Invoke(context);

                    Storytelling.Data["Response"] = context.Response.StatusCode;
                }
                catch (Exception e)
                {
                    var m = e.Message;
                    Storytelling.Error(m);
                    throw;
                }
            });
        }
    }
```

## Story

### Properties

* **Name** - The name of the story
* **InstanceId** - The id of the story
* **Task** - The task observed by this story (on async)
* **StartDateTime** - The start date time
* **Elapsed** - How long the story ran
* **Data** - The data on this story
* **Log** - The logs of this story
* **Children** - (Sub)stories of this story
* **Parent** - The parent of this story (null if this is a root story)
* **HandlerProvider** - The ruleset that will run when the story starts/stops.

### Substories

Whenever you start a new story and there is already a story in the context, the new story will be added as a child to the current story and the new story will become the current story in the context.

> Note: Rules will run on all stories, even if they are substories, this may lead to unexpected results when outputting the stories to the console for example, the way to work around that is to check whether the story is a root story.

```csharp
    ruleset.Rules.Add(
        new PredicateRule( 
            story => story.IsRoot(), // run only on the root story
            story => StoryHandlers.DefaultConsoleHandler));
```

One scenario that a substory opens is having a (sub)story for an internal operation that may take a while and then have a rule that makes sure this operation doesn't take more than expected

```csharp
    var traceHandler = new TraceHandler("RunQueryTooLong");
    ruleset.Rules.Add(
        new PredicateRule( 
            story => story.Name == "RunQuery" && story.Elapsed.TotalMilliseconds > 5000,
            story => traceHandler));
```

### Story Extensions

Working on a root story that has children can cause some difficulty when trying to access some information that is held by some substory, this is where the story extension methods come into play and should be used inside rules.

```csharp
    // Instead of Data
    value = story.Data["key"];

    // Use GetDataValue which looks for the data recursively within the story and children and returns the first it finds
    value = story.GetDataValue("key");

    // Or use GetDataValues which is similar but returns all values (and corresponding story) with that key
    IEnumerable<StoryDataValue> values = story.GetDataValues("key");
```

Similarly for logs.

```csharp
    // Instead of Log
    log = story.Log;

    // Use GetLogs()
    log = story.GetLog();
```

## Rules

### PredicateRule

Has 2 arguments:

* The predicate: given a story, should this story get handled
* The story handler: how to handle the story

```csharp
    ruleset.Rules.Add(
        new PredicateRule(
            story => story.GetDataValue("userId") as string == "123456",
            story => StoryHandlers.DefaultTraceHandler));
```

### MinimumSeverityRule

Handle stories with at least a single log entry with the specified severity.

```csharp
    // If at least a single log entry had an error severity (or more), then print all log lines (debug severity and up).
    var errorTraceHandler = new TraceHandler("TraceError", StoryFormatters.GetBasicStoryFormatter(LogSeverity.Debug);
    ruleset.Rules.Add(
        new MinimumSeverityRule(
            LogSeverity.Error,
            story => errorTraceHandler)));

```

### Custom Rule

```csharp
    public class CustomRule : IRule<IStory, IStoryHandler>
    {
        private IStoryHandler customStoryHandler = new CustomStoryHandler();

        // When to handle story
        public bool When(IStory story)
        {
            if (story.IsRoot())
            {
                if (story.GetDataValue(StoryKeys.IsTestRequest) != null)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        // The handler to handle the story
        public IStoryHandler Then(IStory story)
        {
            return this.customStoryHandler;
        }
    }
```

## Story Handlers

### ConsoleHandler / TraceHandler

Output the story to the console or the trace using a story formatter (which formats a story to a string):

* `StoryFormatters.GetBasicStoryFormatter(severityThreshold)` (default is BasicStoryFormatter with Info severity threshold).
* `StoryFormatters.GetDelimiterStoryFormatter(severityThreshold)`

### ActionHandler

Provide action delegates to the `ActionHandler` for start and stop events of the story.

```csharp
    var actionHandler =
      new ActionHandler(
        "Action",
        s =>
        {
            Console.WriteLine("Story " + story.Name);
        });

    ruleset.Rules.Add(
        new PredicateRule(
            story => true,
            story => actionHandler));
```

### CompositeHandler

Have more than a single story handler to handle your story for the specific rule.

```csharp
    var storyHandler =
        new CompositeHandler(
            "Composite",
            new ConsoleHandler("Console"),
            new TraceHandler("Trace");)
// Or

    var storyHandler =
        new ConsoleHandler("Console").Compose(
        new TraceHandler("Trace"));

    ruleset.Rules.Add(
        new PredicateRule(
            story => story.IsRoot(),
            story => storyHandler));
```

### Custom Story Handler

To create your own custom story handler simply derive from `StoryHandlerBase` or implement the `IStoryHandler` interface.

### BufferedHandler

A base class for a custom story handler that invokes the `OnStoriesReady` method only after buffering a set number of stories or a set time has passed.

```csharp
    public class CustomHandler : BufferedHandler
    {
        public AnalyticsEventsHandler(string name)
            : base(name, TimeSpan.FromSeconds(5), 20)
        {
        }

        protected override void OnStoriesComplete(IList<IStory> stories)
        {
            Storytelling.StartNew("CustomHandler.OnStoriesComplete", () =>
            {
                try
                {
                    SendStoriesSomeWhere(stories);
                }
                catch (Exception ex)
                {
                    Storytelling.Error("Story handling failed {0}", ex);
                }
            });
        }
    }
```

### AzureTableStorageHandler

Use this story handler to store stories in Azure Table Storage.

> If you use Azure Web Apps, the [Log Browser](http://www.siteextensions.net/packages/websitelogs/) site extension now supports this and can show your stories stored in the Azure Table Storage in a nice and searchable ui, more info about the site extension [here](http://blog.amitapple.com/post/2014/06/azure-website-logging/).

```csharp
    // AzureTableStorageHandler should be created once and not per rule invocation
    // That way it is able to buffer stories before sending them
    var azureTableStorageHandler =
        new AzureTableStorageHandler(
            "AzureTableStorage",
            new AzureTableStorageHandlerConfiguration());

    ruleset.Rules.Add(
        new PredicateRule(
            story => story.IsRoot(),
            story => azureTableStorageHandler));
```

The `AzureTableStorageHandlerConfiguration` class lets you customize the following:

* **BatchSize** - Number of stories to buffer before sending (default - 50)
* **BatchTimeDelay** - Time to buffer stories before sending them (default - 5 seconds)
* **ConnectionString** - The Azure Storage connection string (default - from your configuration connection string named "StoryTableStorage", this is what the ui will use when looking for the stories)
* **TableName** - The table name (default - "Stories")

> Note: When adding a data to a story with this handler, make sure it is (json) serializable, otherwise it'll fail.

## Authors

Story framework is written and maintained by [Israel Chen](https://github.com/israelchen) and [Amit Apple](https://github.com/amitapl).

## License

The MIT License (MIT)

Copyright (c) 2015 The Story authors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
