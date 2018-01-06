
namespace GenericBizRunner
{
    /// <summary>
    /// This is the primary interface to the sync actions
    /// </summary>
    /// <typeparam name="TBizInterface"></typeparam>
    public interface IActionService<TBizInterface>
    {
        /// <summary>
        /// This contains the Status after the BizAction is run
        /// </summary>
        IBizActionStatus Status { get; }

        /// <summary>
        /// This will run a business action that takes and input and produces a result
        /// </summary>
        /// <typeparam name="TOut">The type of the result to return. Should either be the Business logic output type or class which inherits fromm GenericActionFromBizDto</typeparam>
        /// <param name="inputData">The input data. It should be Should either be the Business logic input type or class which inherits form GenericActionToBizDto</param>
        /// <returns>The result, or default(TOut) if there is an error</returns>
        TOut RunBizAction<TOut>(object inputData);

        /// <summary>
        /// This will run a business action that does not take an input but does produces a result
        /// </summary>
        /// <typeparam name="TOut">The type of the result to return. Should either be the Business logic output type or class which inherits fromm GenericActionFromBizDto</typeparam>
        /// <returns>The result, or default(TOut) if there is an error</returns>
        TOut RunBizAction<TOut>(); 

        /// <summary>
        /// This runs a business action which takes an input and returns just a status message
        /// </summary>
        /// <param name="inputData">The input data. It should be Should either be the Business logic input type or class which inherits form GenericActionToBizDto</param>
        /// <returns>status message with no result part</returns>
        void RunBizAction(object inputData);

        /// <summary>
        /// This will return a new class for input. 
        /// If the type is based on a GenericActionsDto it will run SetupSecondaryData on it before hadning it back
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        TDto GetDto<TDto>() where TDto : class, new();

        /// <summary>
        /// This should be called if a model error is found in the input data.
        /// If the provided data is a GenericActions dto it will call SetupSecondaryData 
        /// to reset any data in the dto ready for reshowing the dto to the user.
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        TDto ResetDto<TDto>(TDto dto) where TDto : class;
    }
}