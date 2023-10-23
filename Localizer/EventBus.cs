using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localizer
{
    internal class EventBus
    {
        private static EventBus? _instance;
        public static EventBus Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EventBus();
                return _instance;
            }
        }

        private readonly Dictionary<Type, List<Delegate>> _subscriptions = new Dictionary<Type, List<Delegate>>();
        public void Publish<T>(T publishedEvent)
        {
            if (_subscriptions.ContainsKey(typeof(T)))
            {
                var delegates = _subscriptions[typeof(T)];
                foreach (var @delegate in delegates)
                {
                    @delegate.DynamicInvoke(publishedEvent);
                }
            }
        }

        public void Subscribe<T>(Action<T> action)
        {
            if (_subscriptions.ContainsKey(typeof(T)))
            {
                _subscriptions[typeof(T)].Add(action);
            }
            else
            {
                _subscriptions[typeof(T)] = new List<Delegate> { action };
            }
        }
    }
}
