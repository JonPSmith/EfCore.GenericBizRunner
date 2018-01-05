using System.Threading.Tasks;
using AutoMapper;
using GenericBizRunner.PublicButHidden;
using Microsoft.EntityFrameworkCore;

namespace GenericActions
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
        /// <summary>
        /// Use this to setup any extra data needed when showing the dto to the user for input, e.g. supporting dropdownlists
        /// This is called a) when a dto is created by GetDto , b) when ResetDto is called and c) when the call to the business logic fails
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        internal protected virtual async Task SetupSecondaryDataAsync(DbContext db)
        {
        }

        /// <summary>
        /// This is used to copy the DTO to the biz data. You can override this if the copy requires 
        /// extra data bing added, e.g. from supporting dropdown lists.
        /// Note: Look at AutoMapperSetup method first as that can handle a number of mapping issues
        /// </summary>
        /// <param name="db"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        internal protected virtual async Task<TBizIn> CopyToBizDataAsync(DbContext db, TDtoIn source)
        {
            return Mapper.Map<TBizIn>(source);
        }
    }
}