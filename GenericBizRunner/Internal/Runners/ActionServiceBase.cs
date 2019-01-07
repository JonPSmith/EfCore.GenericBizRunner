// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using GenericBizRunner.Configuration;
using GenericBizRunner.Helpers;
using GenericBizRunner.PublicButHidden;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.Runners
{
    internal abstract class ActionServiceBase
    {
        protected ActionServiceBase(bool requiresSaveChanges, IWrappedBizRunnerConfigAndMappings wrappedConfig)
        {
            RequiresSaveChanges = requiresSaveChanges;
            WrappedConfig = wrappedConfig;
        }

        /// <summary>
        /// This contains info on whether SaveChanges (with validation) should be called after a succsessful business logic has run
        /// </summary>
        private bool RequiresSaveChanges { get; }

        protected IWrappedBizRunnerConfigAndMappings WrappedConfig { get; }

        /// <summary>
        /// This a) handled optional save to database and b) calling SetupSecondaryData if there are any errors
        /// It also makes sure that the runStatus is used at the primary return so that warnings are passed on.
        /// Note: if it did save successfully to the database it alters the message
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bizStatus"></param>
        /// <returns></returns>
        protected void SaveChangedIfRequiredAndNoErrors(DbContext db, IBizActionStatus bizStatus)
        {
            if (!bizStatus.HasErrors && RequiresSaveChanges)
            {
                if (bizStatus.ValidateSaveChanges(WrappedConfig.Config))
                    bizStatus.CombineErrors(db.SaveChangesWithValidation(WrappedConfig.Config));
                else
                {
                    db.SaveChanges();
                }
                WrappedConfig.Config.UpdateSuccessMessageOnGoodWrite(bizStatus, WrappedConfig.Config);
            }
        }

        /// <summary>
        /// This a) handled optional save to database and b) calling SetupSecondaryData if there are any errors
        /// It also makes sure that the runStatus is used at the primary return so that warnings are passed on.
        /// Note: if it did save successfully to the database it alters the message
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bizStatus"></param>
        /// <returns></returns>
        protected async Task SaveChangedIfRequiredAndNoErrorsAsync(DbContext db, IBizActionStatus bizStatus)
        {
            if (!bizStatus.HasErrors && RequiresSaveChanges)
            {
                if (bizStatus.ValidateSaveChanges(WrappedConfig.Config))
                    bizStatus.CombineErrors(await db.SaveChangesWithValidationAsync(WrappedConfig.Config));
                else
                {
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }

                WrappedConfig.Config.UpdateSuccessMessageOnGoodWrite(bizStatus, WrappedConfig.Config);
            }
        }
    }
}