using StorePOS.Domain.Services;

namespace StorePOS.Api.Endpoints;

public static class EndpointHelpers
{
    public static IResult MapResult<T>(ServiceResult<T> res, Func<T, string>? createdLocation = null)
    {
        if (res.IsSuccess)
        {
            return res.StatusCode switch
            {
                201 when createdLocation is not null && res.Data is not null => Results.Created(createdLocation(res.Data), res.Data),
                204 => Results.NoContent(),
                _ => Results.Ok(res.Data)
            };
        }

        if (res.Errors is not null && res.Errors.Count > 0)
            return Results.ValidationProblem(res.Errors);

        return res.StatusCode switch
        {
            404 => Results.NotFound(new { message = res.Message ?? "Not found" }),
            409 => Results.Conflict(new { message = res.Message ?? "Conflict" }),
            _ => Results.BadRequest(new { message = res.Message ?? "Bad request" })
        };
    }
}
