using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Counter
{
    internal class AtomicCounter
    {
        private static AtomicCounter instance;

        private ConcurrentDictionary<String, Counter32> mapCounter32;
        private ConcurrentDictionary<String, Counter64> mapCounter64;

        public static AtomicCounter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AtomicCounter();
                }
                return instance;
            }
        }

        private AtomicCounter() 
        {
            //获取当前环境中处理器的数量
            int numProcs = Environment.ProcessorCount;
            //按照处理器数量的2倍, 预估对列表的同时操作线程数量
            mapCounter32 = new ConcurrentDictionary<string, Counter32>(numProcs * 2, 100);
            mapCounter64 = new ConcurrentDictionary<string, Counter64>(numProcs * 2, 100);
        }
        public int CounterCount
        {
            get { return mapCounter32.Count + mapCounter64.Count; }
        }
        public void Increase32(string key)
        {
            var counter = mapCounter32.GetOrAdd(key, (k) => new Counter32(k));
            counter.Increment();
        }
        public void Decrease32(string key)
        {
            var counter = mapCounter32.GetOrAdd(key, (k) => new Counter32(k));
            counter.Decrement();
        }
        public void Increase32(string key, Int32 num)
        {
            var counter = mapCounter32.GetOrAdd(key, (k) => new Counter32(k));
            counter.Increment(num);
        }
        public void Decrease32(string key, Int32 num)
        {
            var counter = mapCounter32.GetOrAdd(key, (k) => new Counter32(k));
            counter.Decrement(num);
        }
        public void Increase64(string key)
        {
            var counter = mapCounter64.GetOrAdd(key, (k) => new Counter64(k));
            counter.Increment();
        }
        public void Decrease64(string key)
        {
            var counter = mapCounter64.GetOrAdd(key, (k) => new Counter64(k));
            counter.Decrement();
        }
        public void Increase64(string key, Int64 num)
        {
            var counter = mapCounter64.GetOrAdd(key, (k) => new Counter64(k));
            counter.Increment(num);
        }
        public void Decrease64(string key, Int64 num)
        {
            var counter = mapCounter64.GetOrAdd(key, (k) => new Counter64(k));
            counter.Decrement(num);
        }
        public Int32 Reset32(string key)
        {
            var counter = mapCounter32.GetOrAdd(key, (k) => new Counter32(k));
            return counter.Reset();
        }
        public Int64 Reset64(string key)
        {
            var counter = mapCounter64.GetOrAdd(key, (k) => new Counter64(k));
            return counter.Reset();
        }
        public Counter32 Get32(string key)
        {
            Counter32 counter;
            if (!mapCounter32.TryGetValue(key, out counter))
            {
                return null;
            }
            return counter;
        }
        public Counter64 Get64(string key)
        {
            Counter64 counter;
            if (!mapCounter64.TryGetValue(key, out counter))
            {
                return null;
            }
            return counter;
        }
        public IList<KeyValuePair<string, Int64>> GetAll()
        {
            IList<KeyValuePair<string, Int64>> datas = new List<KeyValuePair<string, Int64>>();
            datas.AddRange(mapCounter32.ToArray().Select(o => new KeyValuePair<string, Int64>(o.Key, o.Value.Counter)));
            datas.AddRange(mapCounter64.ToArray().Select(o=>new KeyValuePair<string,Int64>(o.Key, o.Value.Counter)));
            return datas;
        }
        public void Clear()
        {
            mapCounter32.Clear();
            mapCounter64.Clear();
        }
    }
}
