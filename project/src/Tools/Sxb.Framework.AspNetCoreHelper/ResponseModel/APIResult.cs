﻿using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.Utils;
using Sxb.Framework.Foundation;

namespace Sxb.Framework.AspNetCoreHelper.ResponseModel
{
    [Description("通用响应结果")]
    public class APIResult<T> where T : class
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        [Description("是否成功")]
        public bool Succeed { get; set; }

        /// <summary>
        /// 返回时间
        /// </summary>
        [JsonConverter(typeof(HmDateTimeConverter))]
        public DateTime MsgTime => DateTime.Now;

        /// <summary>
        /// 返回错误码
        /// </summary>
        public ResponseCode status { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; }


        /// <summary>
        /// 返回Model
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 返回一个成功的返回值
        /// </summary>
        /// <returns></returns>
        public static ResponseResult Success()
        {
            return Success("操作成功");
        }

        /// <summary>
        /// 返回一个成功的返回值
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseResult Success(string message)
        {
            return Success(null, message);
        }

        /// <summary>
        /// 返回一个成功的返回值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResponseResult Success<TData>(TData data)
        {
            return Success(data, "查询成功");
        }


        /// <summary>
        /// 返回一个操作失败的值
        /// </summary>
        /// <returns></returns>
        public static ResponseResult Failed()
        {
            return Failed(null);
        }

        /// <summary>
        /// 返回一个操作失败的值
        /// </summary>
        /// <returns></returns>
        public static ResponseResult Failed(string msg)
        {
            return Failed(msg, null);
        }

        /// <summary>
        /// 返回一个操作失败的值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResponseResult Failed<TData>(TData data)
        {
            return Failed("操作失败", data);
        }

        /// <summary>
        /// 返回一个操作失败的值
        /// </summary>
        /// <returns></returns>
        public static ResponseResult Failed(string msg, object data)
        {
            return new ResponseResult()
            {
                Succeed = false,
                status = ResponseCode.Failed,
                Msg = msg,
                Data = data
            };
        }

        /// <summary>
        /// 返回一个操作失败的值
        /// </summary>
        /// <returns></returns>
        public static ResponseResult Failed(ResponseCode code, string msg, object data)
        {
            return new ResponseResult()
            {
                Succeed = false,
                status = code,
                Msg = msg,
                Data = data
            };
        }


        /// <summary>
        /// 返回一个操作失败的值
        /// </summary>
        /// <returns></returns>
        public static ResponseResult Failed(ResponseCode code, object data)
        {
            return new ResponseResult()
            {
                Succeed = false,
                status = code,
                Msg = code.Description(),
                Data = data
            };
        }

        /// <summary>
        /// 返回成功的返回值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ResponseResult Success(object data, string msg)
        {
            return new ResponseResult()
            {
                Succeed = true,
                status = ResponseCode.Success,
                Msg = msg,
                Data = data
            };
        }

        public static ResponseResult Build(bool succeed)
        {
            return new ResponseResult()
            {
                Succeed = succeed,
                status = succeed ? ResponseCode.Success : ResponseCode.Failed,
            };
        }

    }
}