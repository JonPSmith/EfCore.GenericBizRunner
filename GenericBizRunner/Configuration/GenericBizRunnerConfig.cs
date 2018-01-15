// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;

namespace GenericBizRunner.Configuration
{
    /// <summary>
    /// This is allows you to configure certain parts of the of the Generic BizRunner.
    /// If you do not provide, or register a class against the IGenericBizRunnerConfig interface, then the default values will be used
    /// </summary>
    public class GenericBizRunnerConfig : IGenericBizRunnerConfig
    {
        /// <summary>
        /// By default the call to SaveChanges will have validation added to it, as its often useful to validate 
        /// the data that your business logic writes to the database 
        /// (Validation inspects data attributes like [MaxLength], plus the IValidatableObject interface if present)
        /// However, validation does take longer, so you can turn it off globally by setting this to true.
        /// Alternatively you can add the IDoNotValidateSaveChanges interface to individual business logic interface definitions
        /// </summary>
        public bool DoNotValidateSaveChanges { get; set; }

        /// <summary>
        /// GenericBizRunner uses a static variable to cache the decoding of bizLogic interfaces to their componnent parts
        /// For normal use this should be false, but for unit testing it should be true 
        /// </summary>
        public bool TurnOffCaching { get; set; }

        /// <summary>
        /// If the business logic does not set a success message then this default message will be returned on a success.
        /// </summary>
        public string DefaultSuccessMessage { get; set; } = "Success.";

        /// <summary>
        /// If the business logic writes to the database and does not set a success message then this default message will be returned on a success.
        /// </summary>
        public string DefaultSuccessAndWriteMessage { get; set; } = "Successfully saved.";

        /// <summary>
        /// If the business logic writes to the database and its success the message does not end with a full stop, 
        /// then this message will be appended this message to the end of the message.
        /// </summary>
        public string AppendToMessageOnGoodWriteToDb { get; set; } = " saved.";


        /// <summary>
        /// This updates the message on a successful write
        /// </summary>
        /// <param name="bizStatus"></param>
        public void UpdateSuccessMessageOnGoodWrite(IBizActionStatus bizStatus)
        {
            if (bizStatus.HasErrors) return;

            if (bizStatus.Message != null && bizStatus.Message == DefaultSuccessMessage)
                bizStatus.Message = DefaultSuccessAndWriteMessage;
            else if (bizStatus.Message.LastOrDefault() != '.' && AppendToMessageOnGoodWriteToDb != null)
                bizStatus.Message += AppendToMessageOnGoodWriteToDb;
        }
    }
}