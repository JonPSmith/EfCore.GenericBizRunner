// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using GenericBizRunner.Configuration;
using GenericBizRunner.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.Runners
{
    internal abstract class ActionServiceBase
    {
        protected ActionServiceBase(WriteToDbStates writeStates, IGenericBizRunnerConfig config)
        {
            WriteStates = writeStates;
            Config = config;
        }

        /// <summary>
        /// This contains info on whether SaveChanges (with validation) should be called after a succsessful business logic has run
        /// </summary>
        private WriteToDbStates WriteStates { get; }

        protected IGenericBizRunnerConfig Config { get; }

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
            if (!bizStatus.HasErrors && WriteStates.HasFlag(WriteToDbStates.WriteToDb))
            {
                if (WriteStates.HasFlag(WriteToDbStates.ValidateWrite))
                    bizStatus.AddValidationResults(db.SaveChangesWithValidation());
                else
                {
                    db.SaveChanges();
                }

                Config.UpdateSuccessMessageOnGoodWrite(bizStatus);
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
            if (!bizStatus.HasErrors && WriteStates.HasFlag(WriteToDbStates.WriteToDb))
            {
                if (WriteStates.HasFlag(WriteToDbStates.ValidateWrite))
                    bizStatus.AddValidationResults(await db.SaveChangesWithValidationAsync());
                else
                {
                    await db.SaveChangesAsync().ConfigureAwait(false);
                }

                Config.UpdateSuccessMessageOnGoodWrite(bizStatus);
            }
        }

        protected static TOut ReturnDefaultAndResetInDto<TOut>(DbContext db, DtoAccessGenerator inDtoAcessor,
            object inputdto)
        {
            inDtoAcessor.SetupSecondaryDataIfRequired(db, inputdto);
            return default(TOut);
        }
    }
}