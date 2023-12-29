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

namespace ToDoList_ListTest
{
    public class DataFactory
    {
        private readonly ApplicationDbContext _dbContext;
        public DataFactory(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public LocalUser CreateTestUser(string username, string password, string role, string name, string email)
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
        public ListTask CreateTestListTask(int id, string title, string category, string description, DateTime dueDate, bool isCompleted)
        {
            var listTask = new ListTask
            {
                Id = id,
                Title = title,
                Category = category,
                Description = description,
                DueDate = dueDate,
                IsCompleted = isCompleted
            };
            _dbContext.ListTasks.Add(listTask);
            _dbContext.SaveChanges();
            return listTask;
        }
        public List<ListTask> CreateTestListTasks()
        {
            var listTasks = new List<ListTask>
            {
                new ListTask
                {
                    Id = 1,
                    Title = "Task1",
                    Category = "Category 1",
                    Description = "Description for Task 1",
                    DueDate = DateTime.MinValue,
                    IsCompleted = false,
                },
                new ListTask
                {
                    Id = 2,
                    Title = "Task 2",
                    Category = "Category 2",
                    Description = "Description for Task 2",
                    DueDate = DateTime.MinValue,
                    IsCompleted = false,
                },
                 new ListTask
                {
                    Id = 3,
                    Title = "Task 3",
                    Category = "Category 3",
                    Description = "Description for Task 3",
                    DueDate = DateTime.MinValue,
                    IsCompleted = false,
                },
                  new ListTask
                {
                    Id = 4,
                    Title = "Task4",
                    Category = "Category 1",
                    Description = "Description for Task 1",
                    DueDate = DateTime.MinValue,
                    IsCompleted = false,
                },
                new ListTask
                {
                    Id = 5,
                    Title = "Task5",
                    Category = "Category 2",
                    Description = "Description for Task 2",
                    DueDate = DateTime.MinValue,
                    IsCompleted = false,
                },
                 new ListTask
                {
                    Id = 6,
                    Title = "Task6",
                    Category = "Category 3",
                    Description = "Description for Task 3",
                    DueDate = DateTime.MinValue,
                    IsCompleted = false,
                },
            };

            _dbContext.ListTasks.AddRange(listTasks);
            _dbContext.SaveChanges();

            return listTasks;
        }

    }
}