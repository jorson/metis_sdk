package metis.client.sdk.entity;

/**
 * Created by Administrator on 14-9-15.
 */
public class CallStack {
    private String absolutePath = "";
    private String referrerUrl = "";
    private String queryData = "";
    private String formData = "";
    private UserIdentity user;
    private ExceptionData exData;

    public String getAbsolutePath() {
        return absolutePath;
    }

    public void setAbsolutePath(String absolutePath) {
        this.absolutePath = absolutePath;
    }

    public String getReferrerUrl() {
        return referrerUrl;
    }

    public void setReferrerUrl(String referrerUrl) {
        this.referrerUrl = referrerUrl;
    }

    public String getQueryData() {
        return queryData;
    }

    public void setQueryData(String queryData) {
        this.queryData = queryData;
    }

    public String getFormData() {
        return formData;
    }

    public void setFormData(String formData) {
        this.formData = formData;
    }

    public UserIdentity getUser() {
        return user;
    }

    public void setUser(UserIdentity user) {
        this.user = user;
    }

    public ExceptionData getExData() {
        return exData;
    }

    public void setExData(ExceptionData exData) {
        this.exData = exData;
    }
}
