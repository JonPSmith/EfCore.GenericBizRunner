// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace GenericBizRunner
{
    /// <summary>
    /// This is an async Action that takes an input and returns a Task containing the result TOut
    /// </summary>
    /// <typeparam name="TIn">Input to the business logic</typeparam>
    /// <typeparam name="TOut">Output from the business logic</typeparam>
    public interface IGenericActionAsync<in TIn, TOut> : IBizActionStatus
    {
        /// <summary>
        /// Async method containing business logic that will be called
        /// </summary>
        /// <param name="inputData"></param>
        Task<TOut> BizActionAsync(TIn inputData);
    }
}