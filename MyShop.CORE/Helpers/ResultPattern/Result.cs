using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Helpers.ResultPattern
{
    public class Result<T>
    {
        public T? Data { get; set; }
        public Error? Error { get; set; }
        public bool IsSuccess => Error == null;
        public static Result<T> Success(T data)
        {
            return new Result<T> { Data = data };
        }
        
        public static Result<T> Failure(string message, string code)
        {
            return new Result<T> { Error = new Error(message, code) };
        }
    }
}
