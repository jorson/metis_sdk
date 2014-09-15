package metis.client.sdk.appender;

import metis.client.sdk.utility.Arguments;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

/**
 * Created by Wuhy on 14-9-15.
 */
public class GathererHttpContext {

    private static final String TAG = "GathererHttpContext";
    private static final Logger logger = LoggerFactory.getLogger(GathererHttpContext.class);

    private static final ThreadLocal<HttpServletRequest> requestHolder = new ThreadLocal<HttpServletRequest>();
    private static final ThreadLocal<HttpServletResponse> responseHolder = new ThreadLocal<HttpServletResponse>();

    private static final ThreadLocal<Boolean> httpAvailable = new ThreadLocal<Boolean>();
    static  {
        httpAvailable.set(Boolean.FALSE);
    }

    public static boolean isAvailable() {
        Boolean available = httpAvailable.get();
        return available.booleanValue();
    }

    public static HttpServletRequest getRequest() {
        HttpServletRequest request = requestHolder.get();
        return request;
    }

    public static HttpServletResponse getResponse() {
        HttpServletResponse response = responseHolder.get();
        return response;
    }

    public static void setRequest(HttpServletRequest request) {
        Arguments.notNull(request);
        if(logger.isInfoEnabled()) {
            logger.info(TAG, "set request:" + request);
        }
        requestHolder.set(request);
        httpAvailable.set(Boolean.TRUE);
    }

    public static void setResponse(HttpServletResponse response) {
        Arguments.notNull(response);
        if(logger.isInfoEnabled()) {
            logger.info(TAG, "set response:" + response);
        }
        responseHolder.set(response);
        httpAvailable.set(Boolean.TRUE);
    }
}
