package metis.client.sdk.sender;

import metis.client.sdk.SingleSender;
import metis.client.sdk.entity.LogEntity;

import java.util.Map;
import java.util.concurrent.ConcurrentLinkedQueue;

/**
 * Created by Administrator on 14-9-5.
 */
public class BaseHttpSender implements SingleSender {

    protected int maxLogEntry = 5000;
    protected int interval = 3;

    protected final int batchSize = 200;
    protected final int sendBatchSize = 500;

    protected ConcurrentLinkedQueue<LogEntity> logList = new ConcurrentLinkedQueue<LogEntity>();
    protected String gathererPath = "";

    @Override
    public void prepare(Map<String, Object> config) {

    }

    @Override
    public void doAppend(LogEntity entry) {

    }

    @Override
    public void clear() {

    }
}
