package metis.client.sdk.counter;

import java.util.concurrent.atomic.AtomicLong;

/**
 * Created by Administrator on 14-8-29.
 */
class Counter64 {

    private String key;
    private String group;
    private AtomicLong counter;

    public Counter64(String key, String group){
        if("".equals(key)){
            throw new IllegalArgumentException(key);
        }
        this.key = key;
        this.group = group;
    }

    public Counter64(String key){
        this(key, "none");
    }

    public String getKey() {
        return key;
    }

    public String getGroup() {
        return group;
    }

    public long getCounter() {
        return counter.get();
    }

    public long increment(){
        return counter.addAndGet(1L);
    }

    public long decrement() {
        return counter.decrementAndGet();
    }

    public boolean reset() {
        return counter.compareAndSet(0L, 0L);
    }
}
