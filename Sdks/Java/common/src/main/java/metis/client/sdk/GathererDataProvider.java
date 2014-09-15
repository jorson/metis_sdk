package metis.client.sdk;

import java.util.Map;

/**
 * Created by Administrator on 14-9-15.
 */
public interface GathererDataProvider {
    //获取而外的数据
    Map<String, Object> getExtendData();
    //获取调用SDK应用的Accesstoken数据
    String getAccesstoken();
}
