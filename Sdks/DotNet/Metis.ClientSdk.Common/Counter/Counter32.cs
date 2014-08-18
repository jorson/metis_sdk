using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Metis.ClientSdk.Counter
{
    public class Counter32
    {
        private string key;
        private string grouping;
        private Int32 counter = 0;

        internal Counter32(string key)
            : this(key, String.Empty)
        {
        }
        internal Counter32(string key, string grouping)
        {
            if (String.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(key);
            this.key = key;
            this.grouping = String.IsNullOrEmpty(grouping) ? "none" : grouping;
        }

        public string Key { get { return key; } }
        public string Grouping { get { return grouping; } }
        public Int32 Counter { get { return counter; } }
        internal int Increment()
        {
            return Interlocked.Increment(ref counter);
        }
        internal int Increment(Int32 num)
        {
            return Interlocked.Add(ref counter, num);
        }
        internal void Decrement()
        {
            Interlocked.Decrement(ref counter);
        }
        internal void Decrement(Int32 num)
        {
            Interlocked.Add(ref counter, 0 - num);
        }
        internal Int32 Reset()
        {
            Int32 reset = 0;
            return Interlocked.Exchange(ref counter, reset);
        }
    }
}
