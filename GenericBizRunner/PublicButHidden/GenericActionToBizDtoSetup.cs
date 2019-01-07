// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using AutoMapper;

namespace GenericBizRunner.PublicButHidden
{
    /// <summary>
    /// This is used to find all ToBiz Dtos
    /// </summary>
    public interface IGenericActionToBizDto { }

    /// <summary>
    /// This is the abstract class that is used by GenericActionToBizDto class
    /// It uses the AutoMapper Profile class to allow creation of the mapping in the ctor
    /// </summary>
    /// <typeparam name="TBizIn"></typeparam>
    /// <typeparam name="TDtoIn"></typeparam>
    public abstract class GenericActionToBizDtoSetup<TBizIn, TDtoIn> : IGenericActionToBizDto
        where TBizIn : class
        where TDtoIn : GenericActionToBizDtoSetup<TBizIn, TDtoIn>
    {
        /// <summary>
        /// Override this to provide your own IMappingExpression to the TBizOut to TDtoOut mapping
        /// </summary>
        internal virtual Action<IMappingExpression<TBizIn, TDtoIn>> MappingConfig { get { return null; } }
    }
}