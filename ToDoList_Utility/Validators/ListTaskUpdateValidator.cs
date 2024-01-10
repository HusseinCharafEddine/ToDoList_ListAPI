using FluentValidation;
using System;
using ToDoList_Utility.Models;
using ToDoList_Utility.Models.DTO;

namespace ToDoList_Utility.Validators
{
    public class ListTaskUpdateValidator : AbstractValidator<ListTaskUpdateDTO>
    {
        public ListTaskUpdateValidator() {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id can not be non positive");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title can not be empty.");
            RuleFor(x => x.Category).MinimumLength(3).WithMessage("Category must be at least 3 characters long.");

            // Custom rule for grouping errors
            RuleFor(x => x).Custom((dto, context) =>
            {
                var errorMessages = new List<string>();

                if (string.IsNullOrEmpty(dto.Title))
                {
                    errorMessages.Add("Title is required.");
                }

                if (dto.Category?.Length < 3)
                {
                    errorMessages.Add("Category must be at least 3 characters long.");
                }

                if (errorMessages.Any())
                {
                    var combinedErrorMessage = $"Invalid input for the following fields: {string.Join(", ", errorMessages)}";
                    context.AddFailure(combinedErrorMessage);
                }
            });

        }
    }
}
