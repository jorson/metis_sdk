using Metis.ClientSdk.ConfigSection;
using Metis.ClientSdk.Entities;
using Metis.ClientSdk.Sender;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Metis.ClientSdk
{
    public sealed class GathererContext
    {
        static GathererContext context;
        //是否已经初始化
        static bool hasInited = false;
        //标记调试模式
        static bool isDebug = false;
        //控制台是否可用
        static bool consoleEnabled = false;
        //日志收集服务的路径
        static string gathererPath = String.Empty;
        //日志手机服务的主机
        static string gathererHost = String.Empty;
        //固定的终端ID
        static int terminalId = 1001;
        //日志发送者; TODO: 这个东西先写在Context, 这样所有的Gatherer都必须用同一种Sender,
        //这显然是反人类的, 后续重构考虑将Sender移到Gatherer
        static ISingleSender sender;
        //JSON序列化
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        static GathererContext()
        {
            //这里初始化通用的配置
            var commonNode = GathererSection.Instance.TryGetNode("common.setting");
            gathererPath = commonNode.TryGetNode("url").Attributes["value"];
            if (String.IsNullOrWhiteSpace(gathererPath))
                throw new ArgumentException("缺少 common.setting/url 配置项");

            gathererHost = commonNode.TryGetNode("host").Attributes["value"];
            if (String.IsNullOrWhiteSpace(gathererHost))
                throw new ArgumentException("缺少 common.setting/host 配置项");

            var strDebug = commonNode.TryGetNode("debug").Attributes["value"];
            Boolean.TryParse(strDebug, out isDebug);
            var strConsoleEnable = commonNode.TryGetNode("console").Attributes["value"];
            Boolean.TryParse(strConsoleEnable, out consoleEnabled);
            //初始化Sender对象
            var senderType = commonNode.TryGetNode("sender").Attributes["value"];
            if (String.IsNullOrWhiteSpace(senderType))
                throw new ArgumentException("缺少 common.setting/sender 配置项");
            sender = SenderFactory.Instance.GetSender(senderType);
        }
        /// <summary>
        /// 以配置的方式初始化Gatherer的上下文
        /// </summary>
        public static void Setup()
        {
            if (hasInited)
                return;

            context = new GathererContext();
            hasInited = true;
        }
        /// <summary>
        /// 初始化Gatherer的上下文
        /// </summary>
        /// <param name="appAccesstoken">应用的AccessToken</param>
        /// <param name="terminalId">默认使用终端的ID</param>
        public static void Setup(string appAccesstoken, int tId)
        {
            if (hasInited)
                return;

            if (String.IsNullOrWhiteSpace(appAccesstoken))
                throw new ArgumentException("应用的AccessToken不能为空");

            terminalId = tId;

            context = new GathererContext();
            hasInited = true;
        }

        public static GathererContext Current { get { return context; } }
        /// <summary>
        /// 标记是否为Debug模式
        /// </summary>
        public bool IsDebug { get { return isDebug; } }
        /// <summary>
        /// 标记控制台是否可用
        /// </summary>
        public bool ConsoleEnabled { get { return consoleEnabled; } }
        /// <summary>
        /// 采集者的地址
        /// </summary>
        public string GathererPath { get { return gathererPath; } protected set { gathererPath = value; } }
        /// <summary>
        /// 采集者主机
        /// </summary>
        public string GathererHost { get { return gathererHost; } protected set { gathererHost = value; } }
        /// <summary>
        /// 固定的终端ID
        /// </summary>
        public int TerminalId { get { return terminalId; } protected set { terminalId = value; } }

        #region 内部发送日志的方法

        /// <summary>
        /// 添加登陆的日志
        /// </summary>
        /// <param name="userId">登陆用户Id</param>
        /// <param name="loginUrl">登陆来源地址</param>
        internal void AppendLogin(long userId, string loginUrl)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 添加注册的日志
        /// </summary>
        /// <param name="accesstoken">当前注册用户的Accesstoken</param>
        /// <param name="registerMode">注册的模式</param>
        public void AppendRegister(int registerMode, string accesstoken)
        {
            //获取应用的AccessToken
            if (!HttpContext.Current.IsAvailable())
                return;
            if (String.IsNullOrEmpty(accesstoken))
                return;

            RegisterEntity entry = new RegisterEntity();
            entry.AccessToken = accesstoken;
            entry.RegisterMode = registerMode;

            if (isDebug)
            {
                GathererLogger.Instance.Write(String.Format("Token:{0},RegisterMode:{1}",
                    accesstoken, registerMode));
            }

            sender.DoAppend(entry);
        }
        /// <summary>
        /// 添加资源使用的日志
        /// </summary>
        /// <param name="fromApp">调用资源接口的APPId</param>
        /// <param name="resourceId">资源Id</param>
        /// <param name="resourceType">资源类型</param>
        /// <param name="userId">使用资源的用户Id</param>
        /// <param name="clientId">使用资源的终端Id</param>
        internal void AppendResourceUse(int fromApp, int resourceId, int resourceType,
    long userId, int clientId = 0)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 添加API调用日志
        /// </summary>
        /// <param name="callUrl">被调用API的地址</param>
        /// <param name="statusCode">调用结果状态码</param>
        /// <param name="callAppAccesstoken">调用接口的APP的AccessToken</param>
        /// <param name="requestSize">请求大小</param>
        /// <param name="responseTime">接口请求耗时</param>
        /// <param name="responseSize">接口请求大小</param>
        internal void AppendApiCall(string accesstoken, string callAccesstoken, int callAppId, 
            string callUrl, int statusCode,
            long responseTime = 0, long requestSize = 0, long responseSize = 0)
        {
            try
            {
                ApiCallEntity entry = new ApiCallEntity
                {
                    AccessToken = accesstoken,
                    //这里做一个修改, 如果通过上面的方式都无法获取,则只使用AccessToken
                    CallAccessToken = String.IsNullOrEmpty(callAccesstoken) ?
                        accesstoken : callAccesstoken,
                    CallAppId = callAppId,
                    CallUrl = callUrl,
                    StatusCode = statusCode,
                    RequestSize = requestSize,
                    ResponseSize = responseSize,
                    ResponseTime = responseTime,
                    TerminalCode = 1001
                };

                if (isDebug)
                {
                    GathererLogger.Instance.Write(String.Format("(API_CALL)Token:{0},CallAppId:{1},CallAccessToken:{2},CallUrl:{3},CallTimestamp:{4},IpAddress:{5}",
                        accesstoken, entry.CallAppId,
                        entry.CallAccessToken, callUrl,
                        entry.CallTimestamp.ToString(),
                        entry.IpAddress));
                }
                sender.DoAppend(entry);
            }
            catch(Exception ex)
            {
                if (isDebug)
                {
                    GathererLogger.Instance.Write(String.Format("(API_CALL)Exception Occour! CallUrl:{0}, Message: {1}, Call Stack:{2}", callUrl, ex.Message, ex.StackTrace));
                    if (HttpContext.Current != null)
                    {
                        String queryString = String.Empty;
                        foreach(string key in HttpContext.Current.Request.QueryString.AllKeys)
                        {
                            queryString += String.Format("{0}={1}&", key, HttpContext.Current.Request.QueryString[key]);
                        }
                        GathererLogger.Instance.Write(String.Format("QueryString:{0}", queryString));
                    }
                }
            }

        }
        /// <summary>
        /// 添加页面访问日志信息
        /// </summary>
        /// <param name="accesstoken">访问用户的AccessToken</param>
        /// <param name="visitePage">访问的页面</param>
        /// <param name="referPage">被应用的页面</param>
        /// <param name="pageParams">页面参数</param>
        internal void AppendPageVisit(string accesstoken, string visitePage, string referPage, NameValueCollection pageParams)
        {
            try
            {
                //获取应用的AccessToken
                if (!HttpContext.Current.IsAvailable())
                    return;
                //如果获取到的AccessToken为空, 本次的请求就放弃掉
                if (String.IsNullOrEmpty(accesstoken))
                    return;
                
                PageVisitEntity entry = new PageVisitEntity();
                entry.ParsePageParams(pageParams);
                entry.AccessToken = accesstoken;
                entry.VisitPage = visitePage;
                entry.ReferPage = referPage;

                if (isDebug)
                {
                    GathererLogger.Instance.Write(String.Format("(PAGE_VISITE)Token:{0},VisitPage:{1},ReferPage:{2},CallTimestamp:{3}",
                        accesstoken, entry.VisitPage,
                        entry.ReferPage, entry.CallTimestamp.ToString()));
                }
                sender.DoAppend(entry);
            }
            catch(Exception ex)
            {
                if(isDebug)
                {
                    GathererLogger.Instance.Write(String.Format("(PAGE_VISITE)Exception Occour! CallUrl:{0}, Message: {1}, Call Stack:{2}", visitePage, ex.Message, ex.StackTrace));
                }
            }
        }
        /// <summary>
        /// 添加异常日志
        /// </summary>
        /// <param name="message">额外异常信息</param>
        /// <param name="ex">异常对象</param>
        /// <param name="accesstoken">accesstoken</param>
        /// <param name="level">日志级别</param>
        public void AppendException(string accesstoken, Exception ex, string message = "", LogLevel level = LogLevel.ERROR)
        {
            SysLogEntity entry = new SysLogEntity()
            {
                LogLevel = level,
                LogMessage = message,
                Logger = "UnhandlerExceptionGatherer"
            };
            //设置AccessToken
            entry.AccessToken = accesstoken;
            CallStack callstack = new CallStack();
            //设置访问上下文的信息
            entry.SetContextInfo(callstack);
            //设置异常相关的信息
            entry.SetExceptionInfo(callstack, ex);
            //序列化对象
            entry.CallInfo = serializer.Serialize(callstack);
            sender.DoAppend(entry);
        }
        /// <summary>
        /// 添加自定义日志类型
        /// </summary>
        /// <param name="entry">自定义的日志实体,<see cref="LogEntity"/></param>
        public void Append(LogEntity entry)
        {
            sender.DoAppend(entry);
        }
        #endregion 

        private void CheckInited()
        {
            if (hasInited)
                throw new ArgumentException("采集器上下文已经初始化, 请不要重复初始化");
        }
    }
}
