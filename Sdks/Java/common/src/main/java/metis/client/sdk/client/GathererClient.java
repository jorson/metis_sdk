package metis.client.sdk.client;

import com.mashape.unirest.http.JsonNode;
import com.mashape.unirest.http.Unirest;
import com.mashape.unirest.http.async.Callback;
import com.mashape.unirest.http.exceptions.UnirestException;
import metis.client.sdk.utility.UriPath;
import org.apache.http.Header;
import org.apache.http.HttpEntity;
import org.apache.http.HttpRequest;
import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpHead;
import org.apache.http.conn.HttpClientConnectionManager;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.impl.client.HttpClientBuilder;
import org.apache.http.impl.conn.PoolingHttpClientConnectionManager;
import org.apache.http.impl.nio.client.CloseableHttpAsyncClient;
import org.apache.http.impl.nio.client.HttpAsyncClientBuilder;
import org.apache.http.message.BasicHeader;
import org.apache.http.nio.client.HttpAsyncClient;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.IOException;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.Future;
import java.util.concurrent.TimeUnit;

/**
 * Created by Administrator on 14-9-10.
 */
public class GathererClient {

    private static final Logger logger = LoggerFactory.getLogger(GathererClient.class);
    private static final String TAG = "GATHERER_CLIENT";

    private int timeout = 2000;
    private Collection<Header> headers = null;

    public void setHost(String host) {
        this.host = host;
    }

    private String host = "";
    private boolean async = false;
    private HttpClientConnectionManager manager;

    public GathererClient(int timeout) {
        this(timeout, new ArrayList<Header>(), null, false);
    }

    public GathererClient(int timeout, Collection<Header> headers, String host, boolean async) {
        this.timeout = timeout;
        this.headers = headers;
        this.host = host;
        this.async = async;
    }

    public void setHeader(String key, String value) {
        Header header = new BasicHeader(key, value);
        this.headers.add(header);
    }

    protected CloseableHttpClient createHttpClient() {
        CloseableHttpClient client = HttpClientBuilder.create()
                .setConnectionManager(manager)
                .setDefaultHeaders(headers)
                .build();
        return client;
    }

    protected CloseableHttpAsyncClient createAsyncHttpClient() {
        CloseableHttpAsyncClient client = HttpAsyncClientBuilder.create()
                .setDefaultHeaders(headers)
                .build();
        return client;
    }

    protected String get(String url)
            throws IOException, UnirestException {
        CloseableHttpClient client = createHttpClient();
        Unirest.setHttpClient(client);
        Unirest.setTimeouts(timeout, timeout);

        try {
            //发起同步的请求
            com.mashape.unirest.http.HttpResponse<String> response
                    = Unirest.get(url).asString();
            return response.getBody();
        } finally {
            Unirest.shutdown();
        }
    }

    protected String getJson(String url)
            throws UnirestException {
        CloseableHttpClient client = createHttpClient();
        Unirest.setHttpClient(client);
        Unirest.setTimeouts(timeout, timeout);

        //发起同步的请求
        com.mashape.unirest.http.HttpResponse<JsonNode> response
                = Unirest.get(url).header("accept", "application/json").asJson();
        JsonNode result = response.getBody();
        return result.toString();
    }

    protected void getJsonAsync(String url)
            throws ExecutionException, InterruptedException {
        CloseableHttpAsyncClient client = createAsyncHttpClient();
        Unirest.setAsyncHttpClient(client);
        Unirest.setTimeouts(timeout, timeout);
        //发起异步的Get请求
        Future<com.mashape.unirest.http.HttpResponse<JsonNode>> future = Unirest.get(url)
                .header("accept", "application/json")
                .asJsonAsync(new Callback<JsonNode>() {
                    @Override
                    public void completed(com.mashape.unirest.http.HttpResponse<JsonNode> response) {
                        int code = response.getCode();
                        //如果响应码不为200
                        if (code != 200) {
                            if (logger.isWarnEnabled()) {
                                logger.warn(TAG, "Remote server response Code[" + String.valueOf(code) + "]");
                            }
                            return;
                        }
                    }

                    @Override
                    public void failed(UnirestException e) {
                        if (logger.isErrorEnabled()) {
                            logger.error(TAG, e);
                        }
                    }

                    @Override
                    public void cancelled() {
                        if (logger.isInfoEnabled()) {
                            logger.info(TAG, "Request has been cancelled!");
                        }
                    }
                });
        //如果请求已经完成
        if(future.isDone()) {
            future.get().getBody();
        }
    }

    public String httpGet(String url, Map<String, Object> data)
            throws IOException, UnirestException {
        String combinePath = UriPath.appendArguments(url, data);
        return get(combinePath);
    }

    public String httpGet(String url)
            throws IOException, UnirestException {
        return get(url);
    }

    public String httpGetAsync(String url) {
        return null;
    }

    public void httpPost(String url, Map<String, String> data) {

    }
}
