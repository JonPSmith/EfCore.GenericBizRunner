// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace GenericBizRunner
{
    public interface IBizActionStatus
    {
        /// <summary>
        /// Contains list of errors
        /// </summary>
        IImmutableList<ValidationResult> Errors { get; }

        /// <summary>
        /// Is true if there are errors
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// This contains a human readable message telling you whether the business logic succeeded or failed
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Adds an error message.
        /// </summary>
        /// <param name="errorMessage">The message</param>
        /// <param name="propertyNames">A list of properties that this error is associated with. Can be none</param>
        void AddError(string errorMessage, params string[] propertyNames);

        /// <summary>
        /// Adds a ValidationResult to the business logic
        /// </summary>
        /// <param name="validationResult"></param>
        void AddValidationResult(ValidationResult validationResult);

        /// <summary>
        /// Adds a collection of ValidationResults to the business logic
        /// </summary>
        /// <param name="validationResult"></param>
        void AddValidationResults(IEnumerable<ValidationResult> validationResults);
    }
}