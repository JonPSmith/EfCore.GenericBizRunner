// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using AutoMapper;
using GenericBizRunner.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

namespace GenericBizRunner
{
    /// <summary>
    /// This is the class that your async DTOs for input should inherit
    /// </summary>
    /// <typeparam name="TBizIn"></typeparam>
    /// <typeparam name="TDtoIn"></typeparam>
    public abstract class GenericActionToBizDtoAsync<TBizIn, TDtoIn> : GenericActionToBizDtoSetup<TBizIn, TDtoIn>
        where TBizIn : class, new()
        where TDtoIn : GenericActionToBizDtoAsync<TBizIn, TDtoIn>
    {
        // Async method lacks 'await' operators and will run synchronously
        /// <summary>
        /// Use this to setup any extra data needed when showing the dto to the user for input, e.g. supporting dropdownlists
        /// This is called a) when a dto is created by GetDto , b) when ResetDto is called and c) when the call to the business logic fails
        /// </summary>
        /// <param name="db"></param>
        /// <param name="status">You can raise an error, which will stop the biz logic from running</param>
        /// <returns></returns>
#pragma warning disable CS1998
        protected internal virtual async Task SetupSecondaryDataAsync(DbContext db, IBizActionStatus status)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
        }


        /// <summary>
        /// This is used to copy the DTO to the biz data. You can override this if the copy requires 
        /// extra data bing added, e.g. from supporting dropdown lists.
        /// Note: Look at AutoMapperSetup method first as that can handle a number of mapping issues
        /// </summary>
        /// <param name="db"></param>
        /// <param name="mapper"></param>
        /// <param name="source"></param>
        /// <returns></returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected internal virtual async Task<TBizIn> CopyToBizDataAsync(DbContext db, IMapper mapper, TDtoIn source)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return mapper.Map<TBizIn>(source);
        }
    }
}