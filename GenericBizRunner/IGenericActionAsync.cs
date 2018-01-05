#region licence
// =====================================================
// GenericActions Library - Library for running business actions
// Filename: IGenericActionAsync.cs
// Date Created: 2015/01/28
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
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
        Task<TOut> RunActionAsync(TIn inputData);
    }
}