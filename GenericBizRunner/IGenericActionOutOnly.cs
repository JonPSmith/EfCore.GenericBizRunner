// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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
        TOut BizAction();
    }
}