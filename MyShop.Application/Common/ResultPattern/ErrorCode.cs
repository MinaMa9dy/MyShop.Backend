namespace MyShop.Application.Common.ResultPattern
{
    public enum ErrorCode
    {
        VALIDATION_ERROR,
        UNAUTHENTICATED,
        FORBIDDEN,
        NOT_FOUND,
        DUPLICATE_ENTRY,
        RATE_LIMIT_EXCEEDED,
        SERVER_ERROR
    }
}
