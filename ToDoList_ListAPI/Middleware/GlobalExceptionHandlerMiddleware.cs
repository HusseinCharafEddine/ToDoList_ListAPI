using Microsoft.AspNetCore.Diagnostics;
using System;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ToDoList_Utility.Models.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace ToDoList_ListAPI.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly ResourceManager _errorResourceManager;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;

            // Instantiate ResourceManager with the correct resource file
            _errorResourceManager = new ResourceManager("ToDoList_ListAPI.Resources.Exceptions.ErrorMessages", typeof(GlobalExceptionHandlerMiddleware).Assembly);
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BadRequestException ex)
            {
                context.Response.Clear();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorMessageBadRequest = GetErrorMessageForCode(ex.ErrorCode);
                //Add the custom error code (ex.ErrorCode) to the message
                await context.Response.WriteAsync($"{{\"error\": \"{ex.ErrorCode} {errorMessageBadRequest}\"}}");
            }
            catch (NotFoundException ex)
            {
                context.Response.Clear();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                var errorMessage = GetErrorMessageForCode(ex.ErrorCode);
                //Add the custom error code (ex.ErrorCode) to the message
                await context.Response.WriteAsync($"{{\"error\": \"{ex.ErrorCode} {errorMessage}\"}}");
            }
            catch (ValidationException ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                // Get the error message from ValidationResult
                var errorMessage = ex.ValidationResult.ErrorMessage;

                // Create a response message
                await context.Response.WriteAsync($"{{\"error\": \"{errorMessage}\"}}");
            }

            catch (Exception ex)
            {
                context.Response.Clear();
                context.Response.ContentType = "application/json";


                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync($"{{\"error\": \"InternalServerError: {ex}\"}}");
            }
        }

        private string GetErrorMessageForCode(int errorCode)
        {
            var errorMessageKey = $"{errorCode}"; 
            var errorMessage = _errorResourceManager.GetString(errorMessageKey);
            return errorMessage;
        }
    }

    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
