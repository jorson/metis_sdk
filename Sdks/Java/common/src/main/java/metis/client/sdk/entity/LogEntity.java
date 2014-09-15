package metis.client.sdk.entity;

import metis.client.sdk.counter.AtomicCounter;

/**
 * Created by Administrator on 14-9-1.
 */
public abstract class LogEntity {

    protected String logType;
    protected String accessToken;
    protected int terminalCode;
    protected long ipAddress;
    protected long callTimestamp;

    public LogEntity(){

    }

    void ack() {
        AtomicCounter.getInstance().increase32(
                String.format("log_entry_%s_ack", logType),
                String.format("log_entry_%s", logType));
    }

    void fail() {
        AtomicCounter.getInstance().increase32(
                String.format("log_entry_%s_fail", logType),
                String.format("log_entry_%s", logType));
    }
}
