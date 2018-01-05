
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace GenericBizRunner
{
    /// <summary>
    /// This is an Action that takes an input and returns a result of TOut
    /// </summary>
    /// <typeparam name="TIn">Input to the business logic</typeparam>
    /// <typeparam name="TOut">Output from the business logic</typeparam>
    public interface IGenericAction<in TIn, out TOut> : IBizActionStatus
    {
        /// <summary>
        /// Method containing business logic that will be called
        /// </summary>
        /// <param name="inputData"></param>
        TOut RunAction(TIn inputData);
    }
}