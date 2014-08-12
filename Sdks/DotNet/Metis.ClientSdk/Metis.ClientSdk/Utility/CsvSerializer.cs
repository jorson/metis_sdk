using Metis.ClientSdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Metis.ClientSdk
{
    internal class CsvSerializer
    {
        public const string FILED_SEPARATOR = ",";
        public const string LINE_SEPARATOR = "\r\n";

        public CsvSerializer()
        {

        }
        /// <summary>
        /// 将单个LogEntry对象序列化为CSV格式的字符串
        /// </summary>
        public string SerializeSingle<T>(T obj) where T : ILogEntity
        {
            //获取对象中可读写字段的Property信息
            TypeAccessor ta = TypeAccessor.GetAccessor(typeof(T));
            var properties = ta.ReadWriteProperties;
            var values = ta.GetReadWritePropertyValues(obj);
            return GetCsvFormatString(properties, values, true);
        }

        /// <summary>
        /// 将多个LogEntry对象序列化为CSV格式的字符串
        /// </summary>
        public string SerializeMutil<T>(List<T> objList) where T : ILogEntity
        {
            TypeAccessor ta = TypeAccessor.GetAccessor(typeof(T));
            var properties = ta.ReadWriteProperties;
            bool containField = true;
            StringBuilder mutilResult = new StringBuilder();

            foreach (T obj in objList)
            {
                var values = ta.GetReadWritePropertyValues(obj);
                string logData = GetCsvFormatString(properties, values, containField);
                mutilResult.AppendFormat("{0}" + LINE_SEPARATOR, logData);

                if (containField)
                {
                    containField = false;
                }
            }

            return mutilResult.ToString();
        }
        /// <summary>
        /// 将单个LogEntry对象序列化为CSV格式的字符串
        /// </summary>
        public string SerializeSingle<T>(Type realType, T obj) where T : ILogEntity
        {
            //获取对象中可读写字段的Property信息
            TypeAccessor ta = TypeAccessor.GetAccessor(realType);
            var properties = ta.ReadWriteProperties;
            var values = ta.GetReadWritePropertyValues(obj);
            return GetCsvFormatString(properties, values, true);
        }
        /// <summary>
        /// 将多个LogEntry对象序列化为CSV格式的字符串
        /// </summary>
        public string SerializeMutil<T>(Type realType, List<T> objList)
            where T : ILogEntity
        {
            TypeAccessor ta = TypeAccessor.GetAccessor(realType);
            var properties = ta.ReadWriteProperties;
            bool containField = true;
            StringBuilder mutilResult = new StringBuilder();

            foreach (object obj in objList)
            {
                var values = ta.GetReadWritePropertyValues(obj);
                string logData = GetCsvFormatString(properties, values, containField);
                mutilResult.Append(logData + LINE_SEPARATOR);

                if (containField)
                {
                    containField = false;
                }
            }

            return mutilResult.ToString();
        }

        private string GetCsvFormatString(IList<PropertyInfo> properties, object[] values, bool containField)
        {
            StringBuilder fieldBuilder = new StringBuilder(), dataBuilder = new StringBuilder();
            for (int i = 0; i < properties.Count; i++)
            {
                //将日志类型和AccessToken忽略掉
                if (properties[i].Name.Equals("LogType", StringComparison.InvariantCultureIgnoreCase) ||
                   properties[i].Name.Equals("AccessToken", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }
                //只接受String, Int32, Int64, Datetime类型的数据
                Type proType = properties[i].PropertyType;
                //添加Field
                fieldBuilder.Append(properties[i].Name);
                //添加Value
                var v = values[i].ToString();
                if (v != null)
                {
                    //如果是字符串类型
                    if (proType == typeof(String))
                        v = FixToCsvString(v);
                    dataBuilder.Append(v);
                }
                if (i != (properties.Count - 1))
                {
                    if(containField)
                    {
                        fieldBuilder.Append(FILED_SEPARATOR);
                    }                    
                    dataBuilder.Append(FILED_SEPARATOR);
                }
            }

            return containField ?
                String.Format("{0}" + LINE_SEPARATOR + "{1}", fieldBuilder.ToString(), dataBuilder.ToString()) :
                dataBuilder.ToString();
        }

        private string FixToCsvString(string orignal)
        {
            if (orignal.IndexOf('"') > -1)
                orignal = orignal.Replace("\"", "\"\"");
            if (orignal.IndexOf(',') > -1 || orignal.IndexOf('\r') > -1 || orignal.IndexOf('\n') > -1)
                orignal = "\"" + orignal + "\"";

            return orignal;
        }
    }
}
