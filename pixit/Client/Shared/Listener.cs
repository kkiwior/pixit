using System;
using System.Threading.Tasks;

namespace pixit.Client.Shared
{
    public abstract class ListenerBase
    {
    }
    
    public class Listener<T> : ListenerBase
    {
        public Func<T, Task> Handler { get; set; }
    }
}