using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace GenericBizRunner
{
    /// <summary>
    /// This is an Action that returns a status with a result TOut
    /// </summary>
    /// <typeparam name="TOut">Output from the business logic</typeparam>
    public interface IGenericActionOutOnly<out TOut> : IBizActionStatus
    {
        /// <summary>
        /// Method containing business logic that will be called
        /// </summary>
        /// <returns>Returns result. if fails returns default value</returns>
        TOut RunAction();
    }
}