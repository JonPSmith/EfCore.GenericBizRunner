using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace GenericBizRunner
{
    /// <summary>
    /// This is the interface for creating and returning 
    /// </summary>
    public interface IStatusGeneric
    {
        /// <summary>
        /// This holds the list of ValidationResult errors. If the collection is empty, then there were no errors
        /// </summary>
        IImmutableList<ValidationResult> Errors { get; }

        /// <summary>
        /// This is true if any errors have been reistered 
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// On success this returns the message as set by the business logic, or the default messages set by the BizRunner
        /// If there are errors it contains the message "Failed with NN errors"
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// This adds one error to the Errors collection
        /// </summary>
        /// <param name="errorMessage">The text of the error message</param>
        /// <param name="propertyNames">optional. A list of property names that this error applies to</param>
        IStatusGeneric AddError(string errorMessage, params string[] propertyNames);

        /// <summary>
        /// This adds one ValidationResult to the Errors collection
        /// </summary>
        /// <param name="validationResult"></param>
        void AddValidationResult(ValidationResult validationResult);

        /// <summary>
        /// This appends a collection of ValidationResults to the Errors collection
        /// </summary>
        /// <param name="validationResults"></param>
        void AddValidationResults(IEnumerable<ValidationResult> validationResults);

        /// <summary>
        /// This allows statuses to be combined
        /// </summary>
        /// <param name="status"></param>
        void CombineErrors(IStatusGeneric status);

        /// <summary>
        /// This is a simple method to output all the errors as a single string - null if no errors
        /// Useful for feeding back all the errors in a single exception (also nice in unit testing)
        /// </summary>
        /// <param name="separator">if null then each errors is separated by Environment.NewLine, otherwise uses the separator you provide</param>
        /// <returns>a single string with all errors separated by the 'separator' string</returns>
        string GetAllErrors(string separator = null);
    }
}