
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lord.Collect.Common.Helpers
{
    public static class GenericExtension
    {

        /// <summary>
        /// 判断对象是否为空。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object source)
        {
            return source == null ||
                source is DBNull ||
                string.IsNullOrEmpty(source.ToString());
        }

        /// <summary>
        /// 检查可空类型的值是否为空，如果不为空，则调用委托函数返回结果。
        /// </summary>
        /// <typeparam name="TSource">对象类型。</typeparam>
        /// <typeparam name="TReturn">返回的类型</typeparam>
        /// <param name="value">要查检的值。</param>
        /// <param name="func">值不为空时调用的函数。</param>
        public static TReturn AssertNotNull<TSource, TReturn>(this TSource? value, Func<TSource, TReturn> func) where TSource : struct
        {
            if (value != null && func != null)
            {
                return func(value.Value);
            }

            return default(TReturn);
        }

        /// <summary>
        /// 检查对象是否为空，如果不为空，则调用委托方法。
        /// </summary>
        /// <typeparam name="TSource">结构类型。</typeparam>
        /// <param name="value">要查检的值。</param>
        /// <param name="action">值不为空时调用的方法。</param>
        public static void AssertNotNull<TSource>(this TSource value, Action<TSource> action)
        {
            if (value != null && action != null)
            {
                action(value);
            }
        }

        /// <summary>
        /// 检查对象是否为空，如果不为空，则调用委托函数返回结果。
        /// </summary>
        /// <typeparam name="TSource">结构类型。</typeparam>
        /// <typeparam name="TReturn">返回的类型</typeparam>
        /// <param name="value">要查检的值。</param>
        /// <param name="func">值不为空时调用的函数。</param>
        public static TReturn AssertNotNull<TSource, TReturn>(this TSource value, Func<TSource, TReturn> func) where TSource : class
        {
            if (value != null && func != null)
            {
                return func(value);
            }

            return default(TReturn);
        }

        /// <summary>
        /// 尝试释放对象占用的资源。
        /// </summary>
        /// <param name="disobj"></param>
        public static void TryDispose(this object disobj)
        {
            if (disobj is IDisposable p)
            {
                p.Dispose();
            }
        }



        /// <summary>
        /// 将对象转换为 T 类型，如果对象不支持转换，则返回 null。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T As<T>(this object obj) where T : class
        {
            return obj as T;
        }

        /// <summary>
        /// 将对象转换为 T 类型，如果对象不支持转换，则返回 null。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static T As<T>(this object obj, Action<T> action, Action other = null) where T : class
        {
            if (obj is T item)
            {
                action?.Invoke(item);
                return item;
            }
            else
            {
                other?.Invoke();
            }

            return default(T);
        }

        /// <summary>
        /// 将对象转换为 T 类型，如果对象不支持转换，则返回 null。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static V As<T, V>(this object obj, Func<T, V> func) where T : class
        {
            if (obj is T item && func != null)
            {
                return func(item);
            }

            return default(V);
        }

        /// <summary>
        /// 判断对象是否可以转换为 T 类型。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Is<T>(this object obj)
        {
            return obj is T;
        }

        /// <summary>
        /// 输出对象的字符串表示方法，对象为 null 时仍然返回一个字符串。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToStringSafely(this object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }

        #region To

        /// <summary>
        /// 将对象转换为指定的类型。
        /// </summary>
        /// <typeparam name="TSource">对象的类型。</typeparam>
        /// <typeparam name="TTarget">要转换的类型。</typeparam>
        /// <param name="source">源对象。</param>
        /// <param name="defaultValue">转换失败后返回的默认值。</param>
        /// <returns></returns>
        public static TTarget To<TSource, TTarget>(this TSource source, TTarget defaultValue = default(TTarget))
        {
            return (TTarget)ToType(source, typeof(TTarget), defaultValue);
        }


        /// <summary>
        /// 将对象转换为指定的类型。
        /// </summary>
        /// <typeparam name="TTarget">要转换的对象类型。</typeparam>
        /// <param name="value">源对象。</param>
        /// <param name="defaultValue">转换失败后返回的默认值。</param>
        /// <returns></returns>
        public static TTarget To<TTarget>(this object value, TTarget defaultValue = default(TTarget))
        {
            return (TTarget)value.ToType(typeof(TTarget), defaultValue);
        }

        /// <summary>
        /// 将对象转换为指定的类型。
        /// </summary>
        /// <param name="value">源对象。</param>
        /// <param name="conversionType">要转换的对象类型。</param>
        /// <param name="defaultValue">转换失败后返回的默认值。</param>
        /// <param name="mapper">转换器。</param>
        /// <returns></returns>
        public static object ToType(this object value, Type conversionType, object defaultValue = null)
        {
            if (value == null && defaultValue != null)
            {
                return defaultValue;
            }

            if (value.GetType() == conversionType)
            {
                return value;
            }

            try
            {
                if (conversionType.IsEnum)
                {
                    return Enum.Parse(conversionType, value.ToString(), true);
                }

                if (conversionType == typeof(bool?) && Convert.ToInt32(value) == -1)
                {
                    return null;
                }


                if (conversionType == typeof(bool))
                {
                    if (value is string)
                    {
                        var lower = ((string)value).ToLower();
                        return lower == "true" || lower == "t" || lower == "1" || lower == "yes" || lower == "on";
                    }
                    return Convert.ToInt32(value) == 1;
                }

                if (value is bool)
                {
                    if (conversionType == typeof(string))
                    {
                        return Convert.ToBoolean(value) ? "true" : "false";
                    }
                    return Convert.ToBoolean(value) ? 1 : 0;
                }

                if (conversionType == typeof(Type))
                {
                    return Type.GetType(value.ToString(), false, true);
                }

                if (value is Type && conversionType == typeof(string))
                {
                    return ((Type)value).FullName;
                }

                if (typeof(IConvertible).IsAssignableFrom(conversionType))
                {
                    return Convert.ChangeType(value, conversionType, null);
                }

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        #endregion

        /// <summary>
        /// 将对象序列化为 Json 字符串。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj, JsonSerializerSettings settings = null)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static List<List<T>> SplitList<T>(this IList<T> data, int count = 1000)
        {
            var result = new List<List<T>>();
            if (data.Count < count)
            {
                result.Add(data.ToList());
                return result;
            }
            var newList = new List<T>();
            for (int i = 0; i < data.Count(); i++)
            {
                newList.Add(data[i]);
                if (count == newList.Count() || i == data.Count() - 1)
                {
                    result.Add(newList);
                    newList = new List<T>();
                }
            }
            return result;
        }

        public static string TryGetValue(this List<KeyValuePair<string, object>> keyValues, string key)
        {
            var keyValue = keyValues.FirstOrDefault(s => s.Key.ToLower() == key.ToLower());
            if (!keyValue.Equals(default(KeyValuePair<string, object>)))
            {
                return keyValue.Value.ToString();
            }
            return string.Empty;
        }

        public static T GetValue<T>(this Dictionary<string, object> dic, string key, T defaultVal = default(T))
        {
            if (dic != null && dic.ContainsKey(key))
            {
                return dic[key].To<T>();
            }
            else 
            {
                return defaultVal;
            }
          
        }
    }
}
