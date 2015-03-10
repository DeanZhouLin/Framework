using System;

namespace JFx
{
    /// <summary>
    /// 组合一组异常
    /// </summary>
    public class CombinedException : Exception
    {
        /// <summary>
        /// 构造一个组合异常类 <see cref = "CombinedException" />.
        /// </summary>
        /// <param name = "message">异常提示信息</param>
        /// <param name = "innerExceptions">一组内部异常</param>
        public CombinedException(string message, Exception[] innerExceptions)
            : base(message)
        {
            InnerExceptions = innerExceptions;
        }

        /// <summary>
        /// 一组内部异常
        /// </summary>
        public Exception[] InnerExceptions { get; protected set; }
    }
}
