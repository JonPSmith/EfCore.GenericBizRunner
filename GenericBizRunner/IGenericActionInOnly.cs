using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace GenericBizRunner
{
    /// <summary>
    /// This is an Action that takes an input and returns a status
    /// </summary>
    /// <typeparam name="TIn">Input to the business logic</typeparam>
    public interface IGenericActionInOnly<in TIn> : IBizActionStatus
    {
        /// <summary>
        /// Method containing business logic that will be called
        /// </summary>
        /// <param name="inputData"></param>
        void RunAction(TIn inputData);
    }
}