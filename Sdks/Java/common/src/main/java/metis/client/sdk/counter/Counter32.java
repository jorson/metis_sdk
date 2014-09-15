package metis.client.sdk.counter;

import java.util.concurrent.atomic.AtomicInteger;

/**
 * Created by Administrator on 14-8-29.
 */
class Counter32 {

    private String key;
    private String group;
    private AtomicInteger counter;

    public Counter32(String key, String group){
        if("".equals(key)){
            throw new IllegalArgumentException(key);
        }
        this.key = key;
        this.group = group;
    }

    public Counter32(String key){
        this(key, "none");
    }

    public String getKey() {
        return key;
    }

    public String getGroup() {
        return group;
    }

    public int getCounter() {
        return counter.get();
    }

    public int increment(){
        return counter.addAndGet(1);
    }

    public int decrement() {
        return counter.decrementAndGet();
    }

    public boolean reset() {
        return counter.compareAndSet(0, 0);
    }
}
