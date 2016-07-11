using System;

namespace Sample.AspNetCore
{
    /// <summary>
    /// 중복된 키 예외
    /// </summary>
    public class DuplicatedKeyException : Exception
    {
        public DuplicatedKeyException(string message) : base(message)
        {
        }
    }
}
