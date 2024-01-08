using FluentValidation;
using System;
using ToDoList_Utility.Models;
using ToDoList_Utility.Models.DTO;

namespace ToDoList_Utility.Validators
{
    public class ListTaskCreateValidator : AbstractValidator<ListTaskCreateDTO>
    {
        public ListTaskCreateValidator() {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title can not be empty.");
            
        }
    }
}
