// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using GenericBizRunner.Configuration;

namespace GenericBizRunner
{
    /// <summary>
    /// This enum allows you to set whether a SaveChange is validated or not
    /// </summary>
    public enum ValidateOnSaveStates
    {
        /// <summary>
        /// Use the state of the DoNotValidateSaveChanges property in the GenericBizRunnerConfig
        /// </summary>
        UseConfig,
        /// <summary>
        /// Always validate on a SaveChanges
        /// </summary>
        Validate,
        /// <summary>
        /// Do not validate on a SaveChanges
        /// </summary>
        DoNotValidate
    }

    /// <summary>
    /// This abstract class provides all various features for error reporting and oher status items
    /// </summary>
    [DebuggerDisplay("Message = {Message}")]
    public abstract class BizActionStatus : StatusGenericHandler, IBizActionStatus
    {

        /// <summary>
        /// This method returns true if the data added or updated should be validated on calling SaveChanges
        /// It inspects the protected ValidateOnSaveSetting and the config's DoNotValidateSaveChanges property
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool ValidateSaveChanges(IGenericBizRunnerConfig config)
        {
            if (ValidateOnSaveSetting == ValidateOnSaveStates.UseConfig)
                return !config.DoNotValidateSaveChanges;

            return ValidateOnSaveSetting == ValidateOnSaveStates.Validate;
        }

        /// <summary>
        /// This allows you to set whether GenericBizRunner will validate a call to SaveChanges
        /// Its default state is to use the IGenericBizRunnerConfig.DoNotValidateSaveChanges boolean, which defaults to false
        /// </summary>
        protected virtual ValidateOnSaveStates ValidateOnSaveSetting => ValidateOnSaveStates.UseConfig;
    }
}