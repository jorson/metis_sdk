package metis.client.sdk.appender;

import metis.client.sdk.GathererDataProvider;
import metis.client.sdk.SingleSender;
import metis.client.sdk.entity.CallStack;
import metis.client.sdk.entity.LogLevel;
import metis.client.sdk.entity.SysLogEntity;
import metis.client.sdk.sender.SenderFactory;
import metis.client.sdk.utility.Arguments;
import org.apache.log4j.AppenderSkeleton;
import org.apache.log4j.Level;
import org.apache.log4j.spi.LoggingEvent;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import javax.servlet.http.HttpServletRequest;
import java.util.HashMap;
import java.util.Map;

/**
 * Created by Administrator on 14-9-4.
 */
public class ComboAppender extends AppenderSkeleton {

    private static final Logger logger = LoggerFactory.getLogger(ComboAppender.class);
    private static final String TAG = "ComboAppender";

    private String localSender;
    private String remoteSender;
    private String remoteUrl;
    private String remoteHost;
    private String localUrl;
    private String logPrefix;
    private int maxQueueSize;
    private int sendInterval;
    private String extendDataProvider;

    private SingleSender lSender, rSender;
    private GathererDataProvider extDataProvider;

    public ComboAppender() {
        super();
    }

    @Override
    public void activateOptions() {
        init();
        super.activateOptions();
    }

    @Override
    protected void append(LoggingEvent loggingEvent) {
        try {
            SysLogEntity entry = new SysLogEntity();

        }
    }

    @Override
    public void close() {

    }

    @Override
    public boolean requiresLayout() {
        return false;
    }

    private void init() {
        Arguments.notNullOrEmpty(remoteSender, "RemoteSender");

        try {
            //初始化Sender对象
            if(!this.localSender.isEmpty()) {
                lSender = SenderFactory.getInstance().getSender(this.localSender);
            }
            rSender = SenderFactory.getInstance().getSender(this.remoteSender);
            //构建remote sender的配置
            Map<String, Object> remoteSenderConfig = new HashMap<String, Object>();
            remoteSenderConfig.put("GathererPath", this.remoteUrl);
            remoteSenderConfig.put("GathererHost", this.remoteHost);
            remoteSenderConfig.put("MaxQueueSize", this.maxQueueSize);
            remoteSenderConfig.put("SendInterval", this.sendInterval);
            rSender.prepare(remoteSenderConfig);
            //构建local sender配置
            if(localSender != null) {
                Map<String, Object> localSenderConfig = new HashMap<String, Object>();
                localSenderConfig.put("LocalUrl", this.localUrl);
                localSenderConfig.put("LogPrefix", this.logPrefix);
                lSender.prepare(localSenderConfig);
            }
            //初始化额外数据的提供者
            if(!this.extendDataProvider.isEmpty()) {
                Class exDataCls = Class.forName(this.extendDataProvider);
                Object exDataClsObj = exDataCls.newInstance();
                if(exDataClsObj instanceof GathererDataProvider) {
                    this.extDataProvider = (GathererDataProvider)exDataClsObj;
                } else {
                    throw new IllegalArgumentException("ExtendDataProvider not " +
                            "implement GathererDataProvider");
                }
            }
        } catch (Exception e) {
            if(logger.isErrorEnabled()) {
                logger.error(TAG, e);
            }
        }
    }

    private SysLogEntity parseLoggingEvent(LoggingEvent logEvent) {
        if(logEvent == null) {
            throw new IllegalArgumentException("logEvent");
        }
        SysLogEntity entry = new SysLogEntity();
        //基本数据
        entry.setLogMessage(logEvent.getMessage().toString());
        entry.setLogLevel(transLogLevel(logEvent.getLevel()));
        entry.setLogger(logEvent.getLoggerName());
        //异常的错误信息
        CallStack callStack = new CallStack();


    }

    private CallStack setCallStackInfo() {

        //如果没有调用GathererFilter来获取到当前的上下文对象
        if(!GathererHttpContext.isAvailable()) {
            return null;
        }
        CallStack callStack = new CallStack();
        HttpServletRequest request = GathererHttpContext.getRequest();

        String refer = request.getHeader("Referer");

        callStack.setAbsolutePath(request.getRequestURI());
        if(!refer.isEmpty())
            callStack.setReferrerUrl(refer);
        callStack.setQueryData(request.getQueryString());
        if(request.getMethod().equalsIgnoreCase("post")) {
            request.get
        }
    }

    private LogLevel transLogLevel(org.apache.log4j.Level level) {
        if(level == Level.DEBUG) {
            return LogLevel.DEBUG;
        } else if(level == Level.INFO) {
            return LogLevel.INFO;
        } else if(level == Level.WARN) {
            return LogLevel.WARN;
        } else if(level == Level.ERROR) {
            return LogLevel.ERROR;
        } else if(level == Level.FATAL) {
            return LogLevel.FATAL;
        } else {
            return LogLevel.INFO;
        }
    }

    public void setLocalSender(String localSender) {
        this.localSender = localSender;
    }

    public void setRemoteSender(String remoteSender) {
        this.remoteSender = remoteSender;
    }

    public void setRemoteUrl(String remoteUrl) {
        this.remoteUrl = remoteUrl;
    }

    public void setRemoteHost(String remoteHost) {
        this.remoteHost = remoteHost;
    }

    public void setLocalUrl(String localUrl) {
        this.localUrl = localUrl;
    }

    public void setLogPrefix(String logPrefix) {
        this.logPrefix = logPrefix;
    }

    public void setMaxQueueSize(int maxQueueSize) {
        this.maxQueueSize = maxQueueSize;
    }

    public void setSendInterval(int sendInterval) {
        this.sendInterval = sendInterval;
    }

    public void setExtendDataProvider(String extendDataProvider) {
        this.extendDataProvider = extendDataProvider;
    }
}
