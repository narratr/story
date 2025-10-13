using Newtonsoft.Json;

namespace Story.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Web;
    using Utils;

    [Serializable]
    public abstract class ContextBoundObject<T>
    {
        public static string ContextKey = typeof(ContextBoundObject<T>).FullName + "_" + Guid.NewGuid();

        private ConcurrentQueue<ContextBoundObject<T>> _childrenBag = new ConcurrentQueue<ContextBoundObject<T>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextBoundObject{T}"/> class.
        /// </summary>
        /// <param name="instanceId">The instance identifier.</param>
        protected ContextBoundObject()
        {
            this.InstanceId = InstanceIdGenerator.GenerateInstanceId("OBJ");
        }

        /// <summary>
        /// Gets the instance identifier.
        /// </summary>
        public string InstanceId { get; private set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        [JsonIgnore]
        public ContextBoundObject<T> Parent { get; private set; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        public IEnumerable<ContextBoundObject<T>> Children
        {
            get
            {
                return _childrenBag;
            }
        }

        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public void Detach()
        {
            if (HttpContext.Current != null)
            {
                // Use HttpContext to attach this object to since when using ASP.NET, requests can be migrated
                // from the thread-pool to an internal queue and back and it will make us lose our context.
                //
                // see: http://piers7.blogspot.com/2005/11/threadstatic-callcontext-and_02.html
                HttpContext.Current.Items[ContextKey] = this.Parent;
            }
            else
            {
                if (this.Parent != null)
                {
                    CallContext.LogicalSetData(ContextKey, this.Parent);
                }
                else
                {
                    CallContext.FreeNamedDataSlot(ContextKey);
                }
            }

            this.Parent = null;
        }

        /// <summary>
        /// Attaches this instance.
        /// </summary>
        public void Attach()
        {
            if (HttpContext.Current != null)
            {
                // Use HttpContext to attach this object to since when using ASP.NET, requests can be migrated
                // from the thread-pool to an internal queue and back and it will make us lose our context.
                //
                // see: http://piers7.blogspot.com/2005/11/threadstatic-callcontext-and_02.html
                var existingContext = (ContextBoundObject<T>)HttpContext.Current.Items[ContextKey];

                if (existingContext != null)
                {
                    // Check for rotation if this is a Story
                    existingContext = CheckAndRotateIfNeeded(existingContext);

                    this.Parent = existingContext;
                    this.Parent._childrenBag.Enqueue(this);

                    // Increment descendant count on root story
                    IncrementDescendantCountOnRoot(existingContext);
                }

                HttpContext.Current.Items[ContextKey] = this;
            }
            else
            {
                var existingContext = (ContextBoundObject<T>)CallContext.LogicalGetData(ContextKey);

                if (existingContext != null)
                {
                    // Check for rotation if this is a Story
                    existingContext = CheckAndRotateIfNeeded(existingContext);

                    this.Parent = existingContext;
                    this.Parent._childrenBag.Enqueue(this);

                    // Increment descendant count on root story
                    IncrementDescendantCountOnRoot(existingContext);
                }

                CallContext.LogicalSetData(ContextKey, this);
            }
        }

        /// <summary>
        /// Checks if rotation is needed and performs it if necessary.
        /// </summary>
        private ContextBoundObject<T> CheckAndRotateIfNeeded(ContextBoundObject<T> existingContext)
        {
            // Don't check for rotation if we're already in the middle of rotating
            if (Utils.StoryRotationHelper.IsRotating)
            {
                return existingContext;
            }

            // Only perform rotation checks for Story objects
            var story = existingContext as Story;
            if (story == null)
            {
                return existingContext;
            }

            // Get the root story
            var rootStory = story.GetRootStory();
            if (rootStory == null)
            {
                return existingContext;
            }

            // Get limits from Storytelling
            var limits = Storytelling.Limits;

            // Check if rotation is needed
            string reason;
            if (Utils.StoryRotationHelper.ShouldRotate(rootStory, limits, out reason))
            {
                // Perform rotation with the reason - returns new root story
                var newRootStory = Utils.StoryRotationHelper.RotateStory(rootStory, reason);

                // Return the new story (which is now in context)
                return newRootStory as ContextBoundObject<T>;
            }

            return existingContext;
        }

        /// <summary>
        /// Increments the descendant count on the root story and sets the root reference for the child.
        /// </summary>
        private void IncrementDescendantCountOnRoot(ContextBoundObject<T> parent)
        {
            var parentStory = parent as Story;
            var childStory = this as Story;

            if (parentStory != null && childStory != null)
            {
                var rootStory = parentStory.GetRootStory();
                if (rootStory != null)
                {
                    // Set the root story reference for the child
                    childStory.SetRootStory(rootStory);

                    // Increment descendant count on root
                    rootStory.IncrementDescendantCount();
                }
            }
        }

        public static ContextBoundObject<T> FromContext()
        {
            if (HttpContext.Current != null)
            {
                // Use HttpContext to attach this object to since when using ASP.NET, requests can be migrated
                // from the thread-pool to an internal queue and back and it will make us lose our context
                // since ONLY HttpContext is migrated along with the request, not CallContext items.
                //
                // see: http://piers7.blogspot.com/2005/11/threadstatic-callcontext-and_02.html

                return (ContextBoundObject<T>)HttpContext.Current.Items[ContextKey];
            }
            else
            {
                return (ContextBoundObject<T>)CallContext.LogicalGetData(ContextKey);
            }
        }
    }
}
