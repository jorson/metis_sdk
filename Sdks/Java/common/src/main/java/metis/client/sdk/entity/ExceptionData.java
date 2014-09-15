package metis.client.sdk.entity;

/**
 * Created by Administrator on 14-9-15.
 */
public class ExceptionData {
    private String extendMessage = "";
    private String exceptionType = "";
    private String causeMethod = "";
    private String causeSource = "";
    private String errorMessage = "";
    private String traceStack = "";

    public String getExtendMessage() {
        return extendMessage;
    }

    public void setExtendMessage(String extendMessage) {
        this.extendMessage = extendMessage;
    }

    public String getExceptionType() {
        return exceptionType;
    }

    public void setExceptionType(String exceptionType) {
        this.exceptionType = exceptionType;
    }

    public String getCauseMethod() {
        return causeMethod;
    }

    public void setCauseMethod(String causeMethod) {
        this.causeMethod = causeMethod;
    }

    public String getCauseSource() {
        return causeSource;
    }

    public void setCauseSource(String causeSource) {
        this.causeSource = causeSource;
    }

    public String getErrorMessage() {
        return errorMessage;
    }

    public void setErrorMessage(String errorMessage) {
        this.errorMessage = errorMessage;
    }

    public String getTraceStack() {
        return traceStack;
    }

    public void setTraceStack(String traceStack) {
        this.traceStack = traceStack;
    }
}
