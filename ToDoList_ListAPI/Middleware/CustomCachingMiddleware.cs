using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace ToDoList_ListAPI.Middleware
{
    public class CustomCachingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Dictionary<string, (byte[], DateTime)> _cache;

        public CustomCachingMiddleware(RequestDelegate next)
        {
            _next = next;
            _cache = new Dictionary<string, (byte[], DateTime)>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string cacheKey = context.Request.Path;
            if (_cache.TryGetValue(cacheKey, out var cachedResponse))
            {
                if (DateTime.UtcNow < cachedResponse.Item2)
                {
                    _cache[cacheKey] = (cachedResponse.Item1, DateTime.UtcNow.AddSeconds(30));

                    context.Response.Headers.Add("X-Custom-Cache", "HIT");

                    context.Response.Headers.Add("Content-Type", "application/json");

                    context.Response.StatusCode = 200;

                    await context.Response.WriteAsync(Encoding.UTF8.GetString(cachedResponse.Item1));

                    return;
                }
                else
                {
                    _cache.Remove(cacheKey);
                }
            }

            var originalBodyStream = context.Response.Body;
            using (var newBodyStream = new MemoryStream())
            {
                context.Response.Body = newBodyStream;

                await _next(context);

                if (context.Response.StatusCode == 200)
                {
                    var responseBytes = newBodyStream.ToArray();
                    _cache[cacheKey] = (responseBytes, DateTime.UtcNow.AddSeconds(30));

                    newBodyStream.Position = 0;

                    // Copy the response headers to the original response
                    //foreach (var header in context.Response.Headers)
                    //{
                    //    context.Response.Headers[header.Key] = header.Value.ToArray();
                    //}

                    await newBodyStream.CopyToAsync(originalBodyStream);
                }
            }
        }

    }
    public static class CustomCachingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomCaching(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomCachingMiddleware>();
        }
    }
}