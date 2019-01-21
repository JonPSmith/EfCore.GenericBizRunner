// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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