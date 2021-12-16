using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ebox.Core.Model
{
    // <summary>
    /// 返回结果集
    /// </summary>
    public class Result
    {
        internal const string DefaultExceptionMessage = "未知错误！";

        /// <summary>
        /// 构造方法
        /// </summary>
        public Result()
        {
            this.Code = HttpStatusCode.NotFound;
            this.Msg = "操作失败";
            this.Error = "";
            this.Url = "";
        }

        /// <summary>
        /// 返回成功的对象
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Result Success(string msg)
        {
            if (string.IsNullOrEmpty(msg)) msg = "操作成功";
            return new Result
            {
                Code = HttpStatusCode.OK,
                Msg = msg,
                Error = "",
                Url = ""
            };
        }

        /// <summary>
        /// 返回成功的对象
        /// </summary>
        /// <returns></returns>
        public static Result Success()
        {
            return Success(string.Empty);
        }

        /// <summary>
        /// 返回成功的带结果集的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> Success<T>(string msg, T data)
        {
            if (string.IsNullOrWhiteSpace(msg)) msg = "操作成功！";
            return new Result<T>
            {
                Code = HttpStatusCode.OK,
                Msg = msg,
                Error = "",
                Url = "",
                Data = data
            };
        }

        /// <summary>
        /// 返回成功的带结果集的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> Success<T>(T data)
        {
            return Success<T>(string.Empty, data);
        }

        /// <summary>
        /// 返回失败的带结果集的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> Fail<T>(string msg, T data)
        {
            if (string.IsNullOrWhiteSpace(msg)) msg = "操作失败！";
            return new Result<T>
            {
                Code = HttpStatusCode.BadRequest,
                Msg = msg,
                Error = "",
                Url = "",
                Data = data
            };
        }

        /// <summary>
        /// 返回失败的对象
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result Fail(HttpStatusCode code, string msg)
        {
            if (string.IsNullOrWhiteSpace(msg)) msg = "操作失败！";
            return new Result
            {
                Code = code,
                Msg = msg,
                Error = "",
                Url = ""
            };
        }

        /// <summary>
        /// 返回失败的带结果集的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> Fail<T>(HttpStatusCode code, string msg, T data)
        {
            if (string.IsNullOrWhiteSpace(msg)) msg = "操作失败！";
            return new Result<T>
            {
                Code = code,
                Msg = msg,
                Error = "",
                Url = "",
                Data = data
            };
        }

        /// <summary>
        /// 返回失败的带结果集的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result<T> Fail<T>(string msg)
        {
            return Fail<T>(msg, default);
        }

        /// <summary>
        /// 返回失败的对象
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Result Fail(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg)) msg = "操作失败！";
            return new Result
            {
                Code = HttpStatusCode.BadRequest,
                Msg = msg,
                Error = "",
                Url = ""
            };
        }

        #region 扩展方法
        /// <summary>
        /// 异步返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<Result<T>> GetAsync<T>(T data)
        {
            return await Task<Result<T>>.Factory.StartNew(() =>
            {
                try
                {
                    return Success(data);
                }
                catch (Exception c)
                {
                    return Fail<T>(c.ToString());
                }
            });
        }

        /// <summary>
        /// 异步返回结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<Result<T>> GetAsync<T>(string msg, T data)
        {
            return await Task<Result<T>>.Factory.StartNew(() =>
            {
                try
                {
                    return Success(msg, data);
                }
                catch (Exception c)
                {
                    return Fail<T>(c.ToString());
                }
            });
        }

        /// <summary>
        /// 异步返回结果集
        /// </summary>
        /// <returns></returns>
        public static async Task<Result> GetAsync()
        {
            return await Task<Result>.Factory.StartNew(() =>
            {
                try
                {
                    return Success();
                }
                catch (Exception c)
                {
                    return Fail(c.ToString());
                }
            });
        }

        #endregion

        /// <summary>
        /// 状态码
        /// </summary>
        public HttpStatusCode Code { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 错误
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// 错误Url
        /// </summary>
        public string Url { get; set; }

    }


    public class Result<T> : Result
    {
        private T _data = default;

        /// <summary>
        /// 构造方法
        /// </summary>
        public Result()
            : base()
        {
            _data = default;
        }

        /// <summary>
        /// 操作结果业务数据
        /// </summary>
        public T Data
        {
            get
            {
                if (typeof(T).BaseType == typeof(IEnumerator))
                    _data = Activator.CreateInstance<T>();
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        /// <summary>
        /// 返回成功的带消息提示对象
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> Success(string msg, T data)
        {
            return Success<T>(msg, data);
        }

        /// <summary>
        /// 返回成功的带消息提示对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> Success(T data)
        {
            return Success(string.Empty, data);
        }

        /// <summary>
        /// 返回失败的带消息提示对象
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> Fail(string msg, T data)
        {
            return Fail(msg, data);
        }

        /// <summary>
        /// 返回失败的带消息提示对象
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static new Result<T> Fail(string msg)
        {
            return Fail(msg, default);
        }

        public static Result<T> Fail(HttpStatusCode code, string msg, T data)
        {
            return Fail(code, msg, data);
        }

    }
}
