package metis.client.sdk.counter;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

/**
 * Created by Administrator on 14-8-29.
 */
public class AtomicCounter {

    private static volatile AtomicCounter instance;

    private ConcurrentHashMap<String, Counter32> mapCounter32;
    private ConcurrentHashMap<String, Counter64> mapCounter64;

    public static AtomicCounter getInstance() {
        synchronized (AtomicCounter.class) {
            if(instance == null){
                instance = new AtomicCounter();
            }
            return instance;
        }
    }

    private AtomicCounter() {
        //获取当前环境的处理器数量
        int numProcs = Runtime.getRuntime().availableProcessors();
        //初始化队列,默认使用两倍的处理器数量
        mapCounter32 = new ConcurrentHashMap<String, Counter32>(numProcs*2);
        mapCounter64 = new ConcurrentHashMap<String, Counter64>(numProcs*2);
    }

    public int increase32(String key) {
       Counter32 counter = mapCounter32.putIfAbsent(key, new Counter32(key));
       return counter.increment();
    }

    public int  increase32(String key, String group) {
        Counter32 counter = mapCounter32.putIfAbsent(key, new Counter32(key, group));
        return counter.increment();
    }

    public long increase64(String key) {
        Counter64 counter = mapCounter64.putIfAbsent(key, new Counter64(key));
        return counter.increment();
    }

    public long  increase64(String key, String group) {
        Counter64 counter = mapCounter64.putIfAbsent(key, new Counter64(key, group));
        return counter.increment();
    }

    public int decrease32(String key) {
        if(!mapCounter32.containsKey(key))
            return -1;
        return mapCounter32.get(key).decrement();
    }

    public long decrease64(String key) {
        if(!mapCounter64.containsKey(key))
            return -1;
        return mapCounter64.get(key).decrement();
    }

    public void reset32(String key) {
        if(!mapCounter32.containsKey(key))
            return;
        mapCounter32.get(key).reset();
    }

    public void reset64(String key) {
        if(!mapCounter64.containsKey(key))
            return;
        mapCounter64.get(key).reset();
    }

    public void getAll() {
        HashMap<String, HashMap<String, Integer>> datas =
                new HashMap<String, HashMap<String, Integer>>();
    }

    public int counterCount() {
        return mapCounter32.size() + mapCounter64.size();
    }
}
