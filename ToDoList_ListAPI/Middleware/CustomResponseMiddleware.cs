using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ToDoList_ListAPI.Middleware
{
    public class CustomResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using (var memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;

                await _next(context);

                memoryStream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(memoryStream))
                {
                    string responseBody = await reader.ReadToEndAsync();

                    string modifiedResponse = ModifyResponse(responseBody,  context);

                    using (var originalBodyWriter = new StreamWriter(originalBodyStream))
                    {
                        await originalBodyWriter.WriteAsync(modifiedResponse);
                    }
                }
            }
        }

        private string ModifyResponse(string originalResponse, HttpContext context)
        {
            if (string.IsNullOrWhiteSpace(originalResponse))
            {
                return originalResponse;
            }
            try
            {
                string customMessage= "This is a custom response message.";
                if (context.Response.StatusCode == 200)
                {
                    customMessage = "Success!";

                }else if (context.Response.StatusCode == 400)
                {
                    customMessage = "Bad Request";
                }else if (context.Response.StatusCode == 500)
                {
                    customMessage = "Internal Server Error";
                }
                      

                if (TryParseJson(originalResponse))
                {
                    JObject jsonObject = JObject.Parse(originalResponse);

                    jsonObject.Add("customMessage", customMessage);

                    return jsonObject.ToString();
                }
                else
                {
                    return originalResponse;
                }
            }
            catch (Exception ex)
            {
                return originalResponse;
            }
        }


        private bool TryParseJson(string json)
        {
            try
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
