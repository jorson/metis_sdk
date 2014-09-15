package metis.client.sdk.appender;

import javax.servlet.*;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import java.io.IOException;

/**
 * 设置有使用Gatherer的上下文对象
 * Created by wuhy on 14-9-15.
 */
public class GathererContextFilter implements Filter {

    private static final String FILTER_APPLIED = "__gatherer_context_filter__";

    @Override
    public void init(FilterConfig filterConfig) throws ServletException {
    }

    @Override
    public void doFilter(ServletRequest request, ServletResponse response,
                         FilterChain filterChain) throws IOException, ServletException {
        if(request.getAttribute(FILTER_APPLIED) != null) {
            filterChain.doFilter(request, response);
        } else {
            request.setAttribute(FILTER_APPLIED, Boolean.TRUE);
            if(request instanceof HttpServletRequest) {
                GathererHttpContext.setRequest((HttpServletRequest)request);
            }
            if(response instanceof HttpServletResponse) {
                GathererHttpContext.setResponse((HttpServletResponse) request);
            }
            filterChain.doFilter(request, response);
        }
    }

    @Override
    public void destroy() {
    }
}
