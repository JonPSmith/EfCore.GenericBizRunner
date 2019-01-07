// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using GenericBizRunner.Configuration;

namespace GenericBizRunner
{
    /// <summary>
    /// This interface defines all various features for error reporting and status items that the business logic must implement
    /// </summary>
    public interface IBizActionStatus : IStatusGeneric
    {
        /// <summary>
        /// This method is used by GenericBzRunner to work out whether a call to saveChanges should also validate the data
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        bool ValidateSaveChanges(IGenericBizRunnerConfig config);
    }
}