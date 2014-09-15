package metis.client.sdk.entity;

/**
 * Created by Administrator on 14-9-15.
 */
public class UserIdentity {
    private String name = "";
    private boolean isAuthenticated = false;

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public boolean isAuthenticated() {
        return isAuthenticated;
    }

    public void setAuthenticated(boolean isAuthenticated) {
        this.isAuthenticated = isAuthenticated;
    }
}
