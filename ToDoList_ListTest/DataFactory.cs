using NUnit.Framework;
using Moq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ToDoList_ListAPI.Controllers;
using ToDoList_ListAPI.Models.DTO;
using ToDoList_ListAPI.Repository.IRepository;
using ToDoList_ListAPI.Models;
using ToDoList_ListAPI.Data;

[TestFixture]
public class DataFactory
{
    private readonly ApplicationDbContext _dbContext;
    public DataFactory (ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public LocalUser CreateTestUser (string username, string password, string role, string name , string email)
    {
        var user = new LocalUser
        {
            UserName = username,
            Password = password,
            Role = role,
            Name = name, 
            Email = email
        };
        _dbContext.LocalUsers.Add(user);
        _dbContext.SaveChanges();
        return user;
    }
  
}
