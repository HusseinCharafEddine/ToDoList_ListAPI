using FluentValidation;
using System;
using ToDoList_Utility.Models;
using ToDoList_Utility.Models.DTO;

namespace ToDoList_Utility.Validators
{
    public class ListTaskUpdateValidator : AbstractValidator<ListTaskUpdateDTO>
    {
        public ListTaskUpdateValidator() {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title can not be empty.");
            
        }
    }
}
