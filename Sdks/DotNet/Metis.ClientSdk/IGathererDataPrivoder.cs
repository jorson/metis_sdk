using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Metis.ClientSdk
{
    public interface IGathererDataPrivoder
    {
        /// <summary>
        /// 获取SDK无法获取到的日志数据
        /// </summary>
        IDictionary<string, object> GetExtendData(HttpContext context);
        /// <summary>
        /// 获取调用SDK的应用的Accesstoken数据
        /// </summary>
        string GetAccesstoken();
    }
}
