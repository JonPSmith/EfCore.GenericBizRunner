using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace GenericBizRunner
{
    /// <summary>
    /// This is an Action that takes an input and returns a Task
    /// </summary>
    /// <typeparam name="TIn">Input to the business logic</typeparam>
    public interface IGenericActionInOnlyAsync<in TIn> : IBizActionStatus
    {
        /// <summary>
        /// Async method containing business logic that will be called
        /// </summary>
        /// <param name="inputData"></param>
        Task BizActionAsync(TIn inputData);
    }
}