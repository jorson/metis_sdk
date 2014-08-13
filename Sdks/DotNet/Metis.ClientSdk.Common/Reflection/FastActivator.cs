using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Configuration;
using System.ComponentModel;
using System.Reflection;

namespace Metis.ClientSdk
{
    public static class FastActivator
    {
        static Dictionary<Type, Func<object>> factoryCache = new Dictionary<Type, Func<object>>();
        static Dictionary<Type, Func<object[], object>> withArgsFactoryCache = new Dictionary<Type, Func<object[], object>>();

        public static T Create<T>()
        {
            return (T)Create(typeof(T));
        }

        public static object Create(string type)
        {
            return Create(Type.GetType(type));
        }

        public static object Create(Type type)
        {
            if (type == null)
                throw new ArgumentException("type");

            Func<object> f;
            if (!factoryCache.TryGetValue(type, out f))
            {
                lock (factoryCache)
                {
                    if (!factoryCache.TryGetValue(type, out f))
                    {
                        f = Expression.Lambda<Func<object>>(Expression.Convert(Expression.New(type), typeof(object)), new ParameterExpression[0]).Compile();
                        factoryCache[type] = f;
                    }
                }
            }
            return f.Invoke();
        }

        public static object CreateWithArgs(Type type, object[] args)
        {
            if (type == null)
                throw new ArgumentException("type");
            if (args.Length == 0)
                throw new ArgumentException("args");

            Func<object[], object> f;
            if (!withArgsFactoryCache.TryGetValue(type, out f))
            {
                lock (factoryCache)
                {
                    ParameterExpression param = Expression.Parameter(typeof(object[]));
                    if (!withArgsFactoryCache.TryGetValue(type, out f))
                    {
                        f = Expression.Lambda<Func<object[], object>>(
                            Expression.Convert(Expression.New(type), typeof(object)),
                            param).Compile();
                        withArgsFactoryCache[type] = f;
                    }
                }
            }
            return f.Invoke(args);
        }

        //public static object CreateGeneric(Type type)
        //{
        //    if (type == null)
        //        throw new ArgumentException("type");
        //    if(type.IsGenericType
        //}
    }
}
