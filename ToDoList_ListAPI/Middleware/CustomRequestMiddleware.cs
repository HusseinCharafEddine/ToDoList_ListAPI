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
        // Determine the DTO type based on the HTTP method
        string httpMethod = context.Request.Method;
        string dtoType = GetDtoType(httpMethod);

        // Check if the request is related to ListTask and needs validation
        if (IsListTaskRequest(httpMethod, dtoType, context.Request.Path))
        {
            // Deserialize the request based on the DTO type
            object requestDto = null;
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

            switch (dtoType)
            {
                case "ListTaskCreateDTO":
                    requestDto = JsonConvert.DeserializeObject<ListTaskCreateDTO>(requestBody);
                    break;
                case "ListTaskUpdateDTO":
                    requestDto = JsonConvert.DeserializeObject<ListTaskUpdateDTO>(requestBody);
                    break;
                default:
                    // Handle unknown DTO type
                    context.Response.StatusCode = 400;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "Unknown DTO type" }));
                    return;
            }

            // Validate the request using FluentValidation
            var validator = GetValidator(dtoType);
            var validationResult = validator.Validate(new ValidationContext<object>(requestDto));

            if (!validationResult.IsValid)
            {
                // Return a bad request response with validation errors
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(validationResult.Errors));
                return;
            }
        }

        // Continue to the next middleware or controller
        await _next(context);
    }

    private string GetDtoType(string httpMethod)
    {
        switch (httpMethod?.ToUpper())
        {
            case "POST":
                return "ListTaskCreateDTO";
            case "PUT":
                return "ListTaskUpdateDTO";
            // Add other cases for different HTTP methods as needed
            default:
                return "ListTaskDTO";
        }
    }

    private IValidator GetValidator(string dtoType)
    {
        // Implement logic to map DTO type to the corresponding validator
        switch (dtoType)
        {
            case "ListTaskCreateDTO":
                return new ListTaskCreateValidator();
            case "ListTaskUpdateDTO":
                return new ListTaskUpdateValidator();
            default:
                // Handle unknown DTO type
                return null;
        }
    }

    private bool IsListTaskRequest(string httpMethod, string dtoType, string requestPath)
    {
        // Add a condition to check if the request is related to ListTask
        bool isListTaskRelated = requestPath.Contains("/tasks"); // Adjust based on your actual path

        // Check if the request is related to ListTask and needs validation
        return isListTaskRelated &&
               (httpMethod.ToUpper() == "POST" || httpMethod.ToUpper() == "PUT") &&
               (dtoType == "ListTaskCreateDTO" || dtoType == "ListTaskUpdateDTO");
    }
}
