using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToDoList_ListAPI;
using ToDoList_Repository.Data;
using ToDoList_ListAPI.Middleware;
using ToDoList_Repository.Repository;
using ToDoList_Repository.Repository.IRepository;
using ToDoList_Services.Services;
using ToDoList_Services.Services.IServices;
using FluentValidation.AspNetCore;
using FluentValidation;
using ToDoList_Utility.Models.DTO;
using ToDoList_Utility.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});
builder.Services.Configure<KestrelServerOptions>(options =>
{       
    options.AllowSynchronousIO = true;
});


builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(ToDoList_Services.MappingConfig));
builder.Services.AddScoped<IListTaskRepository, ListTaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IListTaskService, ListTaskService>();
builder.Services.AddScoped<IValidator<ListTaskDTO>, ListTaskValidator>();
builder.Services.AddScoped<IValidator<ListTaskCreateDTO>, ListTaskCreateValidator>();
builder.Services.AddScoped<IValidator<ListTaskUpdateDTO>, ListTaskUpdateValidator>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x => {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


//builder.Services.AddControllers(option => {
//    option.CacheProfiles.Add("Default30",
//       new CacheProfile()
//       {
//           Duration = 30
//       });
//    //option.ReturnHttpNotAcceptable=true;
//}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();

});
builder.Services.AddFluentValidationAutoValidation();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    dbContext.Database.Migrate();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CustomRequestMiddleware>();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseMiddleware<CustomResponseMiddleware>();
//app.UseCustomCaching();
app.MapControllers();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();



app.Run();
