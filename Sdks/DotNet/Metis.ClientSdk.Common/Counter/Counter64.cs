using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Metis.ClientSdk.Counter
{
    public class Counter64
    {
        private string key;
        private string grouping;
        private Int64 counter = 0L;

        internal Counter64(string key)
            : this(key, String.Empty)
        {
        }
        internal Counter64(string key, string grouping)
        {
            if (String.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(key);
            this.key = key;
            this.grouping = String.IsNullOrEmpty(grouping) ? "none" : grouping;
        }

        public string Key { get { return key; } }
        public string Grouping { get { return grouping; } }
        public Int64 Counter
        {
            get
            {
                return Interlocked.Read(ref counter);
            }
        }
        internal long Increment()
        {
            return Interlocked.Increment(ref counter);
        }
        internal long Increment(Int64 num)
        {
            return Interlocked.Add(ref counter, num);
        }
        internal void Decrement()
        {
            Interlocked.Decrement(ref counter);
        }
        internal void Decrement(Int64 num)
        {
            Interlocked.Add(ref counter, 0 - num);
        }
        internal Int64 Reset()
        {
            Int64 reset = 0L;
            return Interlocked.Exchange(ref counter, reset);
        }
    }
}
