package metis.client.sdk.sender;

import metis.client.sdk.SingleSender;
import metis.client.sdk.client.GathererClient;
import metis.client.sdk.entity.LogEntity;
import metis.client.sdk.utility.CsvSerializer;
import metis.client.sdk.utility.UriPath;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ConcurrentLinkedQueue;

/**
 * Created by Administrator on 14-9-5.
 */
public class BaseHttpSender implements SingleSender {

    private static final Logger logger = LoggerFactory.getLogger(BaseHttpSender.class);

    protected int maxLogEntry = 5000;
    protected int interval = 3;

    protected final int batchSize = 200;
    protected final int sendBatchSize = 500;

    protected CsvSerializer serializer = new CsvSerializer();
    protected ConcurrentLinkedQueue<LogEntity> logList = new ConcurrentLinkedQueue<LogEntity>();
    protected GathererClient httpClient = new GathererClient(2000);
    protected String gathererPath = "";

    @Override
    public void prepare(Map<String, Object> config) {
        if(config.get("GathererPath") == null)
            throw new IllegalArgumentException("必须存在GathererPath的配置");
        this.gathererPath = String.valueOf(config.get("GathererPath"));
        //设置HttpClient的参数
        Object gathererHost = null;
        if(config.containsKey("GathererHost")) {
            httpClient.setHost(String.valueOf(config.get("GathererHost")));
        }
        httpClient.setHeader("charset", "utf-8");
        //设置其他的变量
        if(config.containsKey("MaxQueueSize")) {
            this.maxLogEntry = Integer.parseInt(config.get("MaxQueueSize").toString());
        }
        if(config.containsKey("SendInterval")) {
            this.interval = Integer.parseInt(config.get("SendInterval").toString());
        }
    }

    @Override
    public void doAppend(LogEntity entry) {
        if(logList.size() < this.maxLogEntry) {
            logList.add(entry);
        }
    }

    @Override
    public void clear() {

    }

    protected void postData(List<LogEntity> logs) {
        try {
            if(logs.size() == 0) {
                return;
            }
            LogEntity firstLog = logs.get(0);
            Map<String, String> data = new HashMap<String, String>();
            data.put("TerminalCode", String.valueOf(firstLog.getTerminalCode()));
            String sendData = serializer.serializeMutil(firstLog.getClass(), logs);
            data.put("MultiData", URLEncoder.encode(sendData, "UTF-8"));
            //获取需要添加的路径
            String postUrl = getRequestUrl(firstLog.getLogType().toLowerCase());
            httpClient.setHeader("AccessToken", firstLog.getAccessToken());
            //向服务器POST数据
            httpClient.httpPost(postUrl, data);
            //响应正确计数
            for(LogEntity log : logs) {
                log.ack();
            }
        } catch(Exception ex) {
            if(logger.isErrorEnabled()) {
                logger.error("HTTPSENDER", ex);
            }
            //响应失败计数
            for(LogEntity log : logs) {
                log.fail();
            }
        }
    }

    private String getRequestUrl(String... args) {
        List<String> frames = new ArrayList<String>();
        frames.add(gathererPath);
        for(String s : args) {
            frames.add(s);
        }
        return UriPath.combine((String[])frames.toArray());
    }
}
