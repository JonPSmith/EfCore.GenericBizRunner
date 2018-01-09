// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using GenericBizRunner.Configuration;

namespace GenericBizRunner
{
    public enum SaveChangesValidationStates
    {
        UseConfig,
        Validate,
        DoNotValidate
    }

    [DebuggerDisplay("Message = {Message}")]
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
            _errors.Add(new ValidationResult
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

        public bool ValidateSaveChanges(IGenericBizRunnerConfig config)
        {
            if (ShouldIValidateSaveChanges == SaveChangesValidationStates.UseConfig)
                return !config.DoNotValidateSaveChanges;

            return ShouldIValidateSaveChanges == SaveChangesValidationStates.Validate;
        }

        /// <summary>
        /// This allows you to set whether GenericBizRunner will validate a call to SaveChanges
        /// Its default state is to use the IGenericBizRunnerConfig.DoNotValidateSaveChanges boolean, which defaults to false
        /// </summary>
        protected virtual SaveChangesValidationStates ShouldIValidateSaveChanges { get; } =
            SaveChangesValidationStates.UseConfig;
    }
}