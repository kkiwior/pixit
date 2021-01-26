using System;
using System.Threading.Tasks;

namespace pixit.Client.Utils
{
    public abstract class ListenerBase
    {
    }
    
    public class Listener<T> : ListenerBase
    {
        public Func<T, Task> Handler { get; set; }
    }
}