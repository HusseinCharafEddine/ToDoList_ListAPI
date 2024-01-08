using Microsoft.AspNetCore.Http;
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

                // Read the response from the memory stream
                using (var reader = new StreamReader(memoryStream))
                {
                    string responseBody = await reader.ReadToEndAsync();

                    // Modify the response as needed
                    string modifiedResponse = ModifyResponse(responseBody);

                    // Write the modified response to the original response stream
                    using (var originalBodyWriter = new StreamWriter(originalBodyStream))
                    {
                        await originalBodyWriter.WriteAsync(modifiedResponse);
                    }
                }
            }
        }

        private string ModifyResponse(string originalResponse)
        {
            if (string.IsNullOrWhiteSpace(originalResponse))
            {
                // Handle empty response
                return originalResponse;
            }
            try
            {
                // Your custom logic to modify the response goes here
                // For example, you can append a custom message to the response

                string customMessage = "This is a custom response message.";

                // Ensure the original response is valid JSON
                if (TryParseJson(originalResponse))
                {
                    // Parse the original response into a JObject
                    JObject jsonObject = JObject.Parse(originalResponse);

                    // Add the custom message to the JSON object
                    jsonObject.Add("customMessage", customMessage);

                    // Serialize the modified JSON object back to string
                    return jsonObject.ToString();
                }
                else
                {
                    // If the original response is not valid JSON, return as is
                    return originalResponse;
                }
            }
            catch (Exception ex)
            {
                // Log the exception during modification
                //Console.WriteLine($"Error during response modification: {ex}");
                //Console.WriteLine($"Original Response Content: {originalResponse}");
                return originalResponse;
            }
        }


        private bool TryParseJson(string json)
        {
            try
            {
                // Attempt to parse the JSON to ensure it's valid
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                return true;
            }
            catch
            {
                // Parsing failed, indicating invalid JSON
                return false;
            }
        }
    }
}
