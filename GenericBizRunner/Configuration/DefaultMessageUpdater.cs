// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;

namespace GenericBizRunner.Configuration
{
    public static class DefaultMessageUpdater
    {
        public static void UpdateSuccessMessageOnGoodWrite(IBizActionStatus bizStatus, IGenericBizRunnerConfig config)
        {
            if (bizStatus.HasErrors) return;

            if (bizStatus.Message != null && bizStatus.Message == config.DefaultSuccessMessage)
                bizStatus.Message = config.DefaultSuccessAndWriteMessage;
            else if (bizStatus.Message.LastOrDefault() != '.' && config.AppendToMessageOnGoodWriteToDb != null)
                bizStatus.Message += config.AppendToMessageOnGoodWriteToDb;
        }
    }
}