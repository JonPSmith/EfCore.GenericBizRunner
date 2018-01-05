namespace GenericBizRunner.Configuration
{
    public interface IGenericBizRunnerConfig
    {
        /// <summary>
        /// By default the call to SaveChanges will have validation added to it, as its often useful to validate 
        /// the data that your business logic writes to the database 
        /// (Validation inspects data attributes like [MaxLength], plus the IValidatableObject interface if present)
        /// However, validation does take longer, so you can turn it off globally by setting this to true.
        /// Alternatively you can add the IDoNotValidateSaveChanges interface to individual business logic interface definitions
        /// </summary>
        bool DoNotValidateSaveChanges { get; set; }

        /// <summary>
        /// GenericBizRunner uses a static variable to cache the decoding of bizLogic interfaces to their componnent parts
        /// For normal use this should be false, but for unit testing it can be true 
        /// </summary>
        bool TurnOffCaching { get; set; }

        /// <summary>
        /// If the business logic does not set a success message then this default message will be returned on a success.
        /// </summary>
        string DefaultSuccessMessage { get; set; }

        /// <summary>
        /// If the business logic writes to the database and does not set a success message then this default message will be returned on a success.
        /// </summary>
        string DefaultSuccessAndWriteMessage { get; set; }

        /// <summary>
        /// If the business logic writes to the database and its success the message does not end with a full stop, 
        /// then this message will be appended this message to the end of the message.
        /// </summary>
        string AppendToMessageOnGoodWriteToDb { get; set; }

        /// <summary>
        /// This updates the message on a successful write
        /// </summary>
        /// <param name="bizStatus"></param>
        void UpdateSuccessMessageOnGoodWrite(IBizActionStatus bizStatus);
    }
}