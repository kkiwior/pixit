﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pixit.Client.Shared
{
    public class Mediator
    {
        private readonly List<ListenerBase> _listeners = new();

        public Task Register<T>(Func<T, Task> handler)
        {
            lock (this._listeners)
            {
                this._listeners.Add(new Listener<T>()
                {
                    Handler = handler,
                });
            }

            return Task.CompletedTask;
        }

        public Task Notify<T>(T data)
        {
            IEnumerable<Listener<T>> eventListeners;

            lock (this._listeners)
            {
                eventListeners = this._listeners.OfType<Listener<T>>();
            }

            return Task.WhenAll(eventListeners.Select(listener => listener.Handler(data)));
        }
    }
}
