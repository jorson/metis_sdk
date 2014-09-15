package metis.client.sdk.utility;

import com.sun.deploy.net.HttpUtils;
import org.apache.commons.lang3.StringUtils;

import java.io.UnsupportedEncodingException;
import java.lang.reflect.Field;
import java.net.URLEncoder;
import java.util.*;

/**
 * Created by Administrator on 14-9-11.
 */
public class UriPath {

    public static final char PathSeparatorChar = '/';

    public static String combine(String... paths) {

        List<String> items = new ArrayList<String>();
        for(String path : paths) {
            if(path.isEmpty()) {
                continue;
            }
            path.replaceAll(String.valueOf(PathSeparatorChar), "");
            if(!path.isEmpty()) {
                items.add(path);
            }
        }
        return StringUtils.join(items, PathSeparatorChar);
    }

    public static String appendArguments(String url, String key, String value)
            throws UnsupportedEncodingException {
        Arguments.notNullOrEmpty(url, "url");
        Arguments.notNullOrEmpty(value, "value");

        if(url.indexOf("?") == -1)
            url += "?";
        else
            url += "&";

        url += key + "=" + URLEncoder.encode(value, "utf-8");
        return url;
    }

    public static String appendArguments(String url, String key, Collection<String> items)
            throws UnsupportedEncodingException {
        for(String item : items) {
            url = appendArguments(url, key, item);
        }
        return url;
    }

    public static String appendArguments(String url, Object value)
            throws IllegalAccessException, UnsupportedEncodingException {
        return appendArguments(url, toMap(value));
    }

    public static String appendArguments(String url, Map<String, Object> data)
           throws UnsupportedEncodingException {
        Arguments.notNullOrEmpty(url, "url");

        if(data != null && data.size() > 0) {
            if(url.indexOf("?") == -1)
                url += "?";
            else
                url += "&";
            url += constructQueryString(data);
        }
        return url;
    }

    private static String constructQueryString(Map<String, Object> data)
            throws UnsupportedEncodingException {
        List<String> items = new ArrayList<String>();

        for(Map.Entry<String, Object> entry : data.entrySet()) {
            items.add(String.format("%s=%s", entry.getKey(),
                    URLEncoder.encode(entry.getValue().toString(), "utf-8")));
        }
        return StringUtils.join(items, "&");
    }

    public static Map<String, Object> toMap(Object value)
            throws IllegalAccessException {

        Map<String, Object> result = new HashMap<String, Object>();

        Class<?> cls = value.getClass();
        //如果类对象是基本类型
        if(cls.isPrimitive()) {
            return null;
        }
        //获取类对象中所有声明的公共字段
        Field[] fields = cls.getFields();
        for(Field field : fields) {
            result.put(field.getName(), field.get(value));
        }
        return result;
    }
}
