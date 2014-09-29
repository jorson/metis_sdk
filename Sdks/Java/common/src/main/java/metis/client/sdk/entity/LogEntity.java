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
        this.callTimestamp = System.currentTimeMillis();
    }

    public String getLogType() {
        return logType;
    }

    public void setLogType(String logType) {
        this.logType = logType;
    }

    public String getAccessToken() {
        return accessToken;
    }

    public void setAccessToken(String accessToken) {
        this.accessToken = accessToken;
    }

    public int getTerminalCode() {
        return terminalCode;
    }

    public void setTerminalCode(int terminalCode) {
        this.terminalCode = terminalCode;
    }

    public long getIpAddress() {
        return ipAddress;
    }

    public long getCallTimestamp() {
        return callTimestamp;
    }

    public void ack() {
        AtomicCounter.getInstance().increase32(
                String.format("log_entry_%s_ack", logType),
                String.format("log_entry_%s", logType));
    }

    public void fail() {
        AtomicCounter.getInstance().increase32(
                String.format("log_entry_%s_fail", logType),
                String.format("log_entry_%s", logType));
    }
}
