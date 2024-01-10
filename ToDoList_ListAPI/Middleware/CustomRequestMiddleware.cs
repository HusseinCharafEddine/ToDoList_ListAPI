using FluentValidation;
using Newtonsoft.Json;
using ToDoList_Utility.Models.DTO;
using ToDoList_Utility.Validators;

public class CustomRequestMiddleware
{
    private readonly RequestDelegate _next;

    public CustomRequestMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        string httpMethod = context.Request.Method;
        var originalBody = context.Request.Body;

        
        await _next(context);
    }

}
