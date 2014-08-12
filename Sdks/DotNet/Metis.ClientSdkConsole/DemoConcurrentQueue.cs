using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdkConsole
{
    public class DemoConcurrentQueue<TEntry> : IProducerConsumerCollection<TEntry>
    {
        private ConcurrentQueue<TEntry> queue;

        public void CopyTo(TEntry[] array, int index)
        {
            throw new NotImplementedException();
        }

        public TEntry[] ToArray()
        {
            throw new NotImplementedException();
        }

        public bool TryAdd(TEntry item)
        {
            throw new NotImplementedException();
        }

        public bool TryTake(out TEntry item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TEntry> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }
    }
}
