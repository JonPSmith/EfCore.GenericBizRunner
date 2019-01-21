// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using AutoMapper;

namespace GenericBizRunner.PublicButHidden
{
    /// <summary>
    /// This is used to find all FromBiz Dtos
    /// </summary>
    internal interface IGenericActionFromBizDto { }

    /// <summary>
    /// This is the abstract class that is used by GenericActionFromBizDto class
    /// It contains a method to alter the AutoMapper mapping configuration
    /// </summary>
    /// <typeparam name="TBizOut"></typeparam>
    /// <typeparam name="TDtoOut"></typeparam>
    public abstract class GenericActionFromBizDtoSetup<TBizOut, TDtoOut> : IGenericActionFromBizDto
        where TBizOut : class
        where TDtoOut : GenericActionFromBizDtoSetup<TBizOut, TDtoOut>
    {
        /// <summary>
        /// Override this to provide your own IMappingExpression to the TBizOut to TDtoOut mapping
        /// </summary>
        protected internal virtual Action<IMappingExpression<TBizOut, TDtoOut>> AlterDtoMapping => null;
    }
}