package metis.client.sdk.entity;

/**
 * Created by Administrator on 14-9-4.
 */
public class SysLogEntity extends LogEntity {

    private LogLevel logLevel = LogLevel.INFO;
    private String logger = "";
    private String logMessage = "";
    private String callInfo = "";

    public SysLogEntity() {
        this.logType = "syslog";
    }

    public LogLevel getLogLevel() {
        return logLevel;
    }

    public void setLogLevel(LogLevel logLevel) {
        this.logLevel = logLevel;
    }

    public String getLogger() {
        return logger;
    }

    public void setLogger(String logger) {
        this.logger = logger;
    }

    public String getLogMessage() {
        return logMessage;
    }

    public void setLogMessage(String logMessage) {
        this.logMessage = logMessage;
    }

    public String getCallInfo() {
        return callInfo;
    }

    public void setCallInfo(String callInfo) {
        this.callInfo = callInfo;
    }
}
