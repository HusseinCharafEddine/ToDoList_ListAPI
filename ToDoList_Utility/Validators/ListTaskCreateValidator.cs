using FluentValidation;
using System;
using System.Collections.Generic;
using ToDoList_Utility.Models.DTO;

namespace ToDoList_Utility.Validators
{
    public class ListTaskCreateValidator : AbstractValidator<ListTaskCreateDTO>
    {
        public ListTaskCreateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title can not be empty.");
            RuleFor(x => x.Category)
                .MinimumLength(3).WithMessage("Category must be at least 3 characters long.");

            // Custom rule for grouping errors
            RuleFor(x => x).Custom((dto, context) =>
            {
                var errorMessages = new List<string>();

                // Check individual properties and add errors to the list
                if (string.IsNullOrEmpty(dto.Title))
                {
                    errorMessages.Add("Title is required.");
                }

                if (dto.Category?.Length < 3)
                {
                    errorMessages.Add("Category must be at least 3 characters long.");
                }

                // If there are errors, combine them and throw a ValidationException
                if (errorMessages.Count > 0)
                {
                    var combinedErrorMessage = $"Invalid input for the following fields: {string.Join(", ", errorMessages)}";
                    throw new ValidationException(combinedErrorMessage);
                }
            });
        }
    }
}
