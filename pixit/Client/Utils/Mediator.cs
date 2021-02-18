using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixit.Client.Utils
{
    public class Mediator
    {
        private readonly List<ListenerBase> _listeners = new();

        public Task Register<T>(Func<T, Task> handler)
        {
            lock (_listeners)
            {
                _listeners.Add(new Listener<T>()
                {
                    Handler = handler,
                });
            }

            return Task.CompletedTask;
        }

        public void Unregister<T>()
        {
            lock (_listeners)
            {
                _listeners.Remove(_listeners.OfType<Listener<T>>().FirstOrDefault());
            }
        }

        public Task Notify<T>(T data)
        {
            IEnumerable<Listener<T>> eventListeners;

            lock (_listeners)
            {
                eventListeners = _listeners.OfType<Listener<T>>();
            }

            return Task.WhenAll(eventListeners.Select(listener => listener.Handler(data)));
        }
    }
}
