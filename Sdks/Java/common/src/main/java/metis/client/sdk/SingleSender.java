package metis.client.sdk;

import metis.client.sdk.entity.LogEntity;

import java.util.Map;

/**
 * Created by Administrator on 14-9-9.
 */
public interface SingleSender {

    void prepare(Map<String, Object> config);
    void doAppend(LogEntity entry);
    void clear();
}
