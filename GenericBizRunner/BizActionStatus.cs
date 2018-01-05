// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GenericBizRunner
{
    public abstract class BizActionStatus : IBizActionStatus
    {
        private readonly List<ValidationResult> _errors = new List<ValidationResult>();
        private string _successMessage;

        public IImmutableList<ValidationResult> Errors => _errors.ToImmutableList();

        public bool HasErrors => _errors.Any();

        public string Message
        {
            get => HasErrors 
                ? $"Failed with {_errors.Count} error" + (_errors.Count == 1 ? "" : "s")
                : _successMessage;
            set => _successMessage = value;
        }

        public void AddError(string errorMessage, params string[] propertyNames)           
        {
            _errors.Add( new ValidationResult  
                (errorMessage, propertyNames));
        }

        public void AddValidationResult(ValidationResult validationResult)
        {
            _errors.Add(validationResult);
        }

        public void AddValidationResults(IEnumerable<ValidationResult> validationResults)
        {
            _errors.AddRange(validationResults);
        }
    }

}