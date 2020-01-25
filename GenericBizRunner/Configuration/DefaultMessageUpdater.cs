// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using StatusGeneric;

namespace GenericBizRunner.Configuration
{
    /// <summary>
    /// This holds the default UpdateSuccessMessageOnGoodWrite implementation
    /// </summary>
    public static class DefaultMessageUpdater
    {
        /// <summary>
        /// This is the default code that changes the Message on successful write to the database
        /// </summary>
        /// <param name="bizStatus"></param>
        /// <param name="config"></param>
        public static void UpdateSuccessMessageOnGoodWrite(IBizActionStatus bizStatus, IGenericBizRunnerConfig config)
        {
            
            if (bizStatus.HasErrors) return;

            if (bizStatus.Message != null && (bizStatus.Message == config.DefaultSuccessMessage || bizStatus.Message == StatusGenericHandler.DefaultSuccessMessage))
                bizStatus.Message = config.DefaultSuccessAndWriteMessage;
            else if (bizStatus.Message.LastOrDefault() != '.' && config.AppendToMessageOnGoodWriteToDb != null)
                bizStatus.Message += config.AppendToMessageOnGoodWriteToDb;
        }
    }
}