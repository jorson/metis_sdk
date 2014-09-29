package metis.client.sdk.sender;

import metis.client.sdk.entity.LogEntity;

import java.util.*;
import java.util.concurrent.*;

/**
 * Created by Administrator on 14-9-5.
 */
public class TimerHttpSender extends BaseHttpSender {

    private ScheduledExecutorService executorService;

    public TimerHttpSender() {
    }

    @Override
    public void prepare(Map<String, Object> config) {
        super.prepare(config);
        executorService = Executors.newScheduledThreadPool(1);
        executorService.scheduleWithFixedDelay(new SendLogTask(), interval, interval, TimeUnit.SECONDS);
    }

    @Override
    public void clear() {
        super.clear();
        if(executorService != null) {
            if(!executorService.isShutdown()) {
                executorService.shutdown();
            }
        }
    }

    private class SendLogTask implements Runnable {
        @Override
        public void run() {
            List<LogEntity> postList = new ArrayList<LogEntity>(100);
            LogEntity entry;
            int counter = 0;

            while (!logList.isEmpty() && counter < sendBatchSize) {
                entry = logList.poll();
                postList.add(entry);
                counter++;
            }

            //如果有日志信息需要被发送
            if(postList.size() > 0) {
                //将日志根据日志类型进行分组
                Map<String, List<LogEntity>> entryGroup = new HashMap<String, List<LogEntity>>();
                for(LogEntity en : postList) {
                    if(!entryGroup.containsKey(en.getLogType())) {
                        List<LogEntity> groupList = new ArrayList<LogEntity>();
                        groupList.add(en);
                        entryGroup.put(en.getLogType(), groupList);
                    } else {
                        entryGroup.get(en.getLogType()).add(en);
                    }
                }

                //按日志类型进行分批发送
                for(Map.Entry<String, List<LogEntity>> gEntries : entryGroup.entrySet()) {
                    int repeatCount = (int)Math.ceil(gEntries.getValue().size() * 1.0D / batchSize);
                    for(int i=0; i<repeatCount; i++) {
                        List<LogEntity> sendList = new ArrayList<LogEntity>();
                        if((i+1) * batchSize > gEntries.getValue().size()) {
                            sendList = gEntries.getValue().subList(i * batchSize,
                                    gEntries.getValue().size()-1);
                        } else {
                            sendList = gEntries.getValue().subList(i * batchSize,
                                    (i+1) * batchSize);
                        }
                        postData(sendList);
                    }
                }
            }
        }
    }
}
