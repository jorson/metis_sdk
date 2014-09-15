package metis.client.sdk.sender;

import metis.client.sdk.SingleSender;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;


/**
 * Created by Administrator on 14-9-5.
 */
public class SenderFactory {

    private static volatile SenderFactory instance;
    private static Logger logger = LoggerFactory.getLogger(SenderFactory.class);
    private final String TAG = "SenderFactory";

    public static SenderFactory getInstance() {
        if(instance == null) {
            instance = new SenderFactory();
        }
        return instance;
    }

    private SenderFactory() {

    }

    public SingleSender getSender(Class<? extends SingleSender> cls) {
        try {
            return cls.newInstance();
        } catch (Exception e) {
            if(logger.isErrorEnabled()) {
                logger.error(TAG, e);
            }
        }
        return null;
    }

    public SingleSender getSender(String clsName)
            throws ClassNotFoundException {
        Class cls = Class.forName(clsName);
        return getSender(cls);
    }
}
