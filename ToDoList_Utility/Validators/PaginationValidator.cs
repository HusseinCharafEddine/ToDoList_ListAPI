using FluentValidation;
using System;
using ToDoList_Utility.Models;
using ToDoList_Utility.Models.DTO;

namespace ToDoList_ListAPI.Validators
{
    public class PaginationValidator : AbstractValidator<Pagination>
    {
        public PaginationValidator() {
            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(0).WithMessage("Page size can not be nonpositive."); 
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(0).WithMessage("Page number can not be nonpositive.");
        }
    }
}
