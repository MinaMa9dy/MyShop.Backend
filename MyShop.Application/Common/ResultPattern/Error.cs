namespace MyShop.Application.Common.ResultPattern
{
    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string[]>? Details { get; set; }  // for validation errors

        public Error(string message, string code, Dictionary<string, string[]>? details = null)
        {
            Message = message;
            Code = code;
            Details = details;
        }
    }
}
