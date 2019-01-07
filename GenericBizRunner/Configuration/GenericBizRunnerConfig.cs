// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GenericBizRunner.Configuration
{
    /// <summary>
    /// This is allows you to configure certain parts of the of the Generic BizRunner.
    /// If you do not provide, or register a class against the IGenericBizRunnerConfig interface, then the default values will be used
    /// </summary>
    public class GenericBizRunnerConfig : IGenericBizRunnerConfig
    {
        /// <inheritdoc />
        public bool DoNotValidateSaveChanges { get; set; }

        /// <inheritdoc />
        public bool TurnOffCaching { get; set; }

        /// <inheritdoc />
        public string DefaultSuccessMessage { get; set; } = "Success.";

        /// <inheritdoc />
        public string DefaultSuccessAndWriteMessage { get; set; } = "Successfully saved.";

        /// <inheritdoc />
        public string AppendToMessageOnGoodWriteToDb { get; set; } = " saved.";

        /// <inheritdoc />
        public Action<IBizActionStatus, IGenericBizRunnerConfig> UpdateSuccessMessageOnGoodWrite { get; set; } =
            DefaultMessageUpdater.UpdateSuccessMessageOnGoodWrite;

        /// <inheritdoc />
        public Func<Exception, DbContext, IStatusGeneric> SaveChangesExceptionHandler { get; set; } = (exception, context) => null; // default is to return null
    }
}