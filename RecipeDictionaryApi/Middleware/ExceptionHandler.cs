using System.Text.Json;

namespace RecipeDictionaryApi.Middleware;

public class ExceptionHandler(RequestDelegate requestDelegate)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await requestDelegate(context);
        }
        catch (Exception e)
        {
            await HandleException(context, e);
        }
    }

    private static Task HandleException(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        
        var errorResponse = new { Message = "Ooops... Something went wrong..." };
        string jsonResponse = JsonSerializer.Serialize(errorResponse);
        
        return context.Response.WriteAsync(jsonResponse);
    }
}