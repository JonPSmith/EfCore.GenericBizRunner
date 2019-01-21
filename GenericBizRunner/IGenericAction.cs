// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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
        TOut BizAction(TIn inputData);
    }
}