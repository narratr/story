using Newtonsoft.Json;

namespace Story.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Web;

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
            this.InstanceId = Guid.NewGuid().ToString();
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
                    this.Parent = existingContext;
                    this.Parent._childrenBag.Enqueue(this);
                }

                HttpContext.Current.Items[ContextKey] = this;
            }
            else
            {
                var existingContext = (ContextBoundObject<T>)CallContext.LogicalGetData(ContextKey);

                if (existingContext != null)
                {
                    this.Parent = existingContext;
                    this.Parent._childrenBag.Enqueue(this);
                }

                CallContext.LogicalSetData(ContextKey, this);
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
