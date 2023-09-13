namespace TaskMagnet.Common.Shared;

public enum ExceptionCodeEnum
{
    Undefined = -1,
    Success = 0,
    NoContent = 200,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    DuplicateValue = 409,
    UnprocessableEntity = 422,
    InternalServerError = 500,
    DuplicateKey = 23505,
    ServiceUnavailable = 503,
}
