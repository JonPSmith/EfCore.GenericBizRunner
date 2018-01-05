using AutoMapper;
using GenericBizRunner.PublicButHidden;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner
{
    /// <summary>
    /// This is the class that your sync DTOs for output should inherit
    /// </summary>
    /// <typeparam name="TBizIn"></typeparam>
    /// <typeparam name="TDtoIn"></typeparam>
    public abstract class GenericActionToBizDto<TBizIn, TDtoIn> : GenericActionToBizDtoSetup<TBizIn, TDtoIn>
        where TBizIn : class, new()
        where TDtoIn : GenericActionToBizDto<TBizIn, TDtoIn>
    {
        /// <summary>
        /// Use this to setup any extra data needed when showing the dto to the user for input, e.g. supporting dropdownlists
        /// This is called a) when a dto is created by GetDto , b) when ResetDto is called and c) when the call to the business logic fails
        /// </summary>
        /// <param name="db"></param>
        protected internal virtual void SetupSecondaryData(DbContext db)
        {
        }

        /// <summary>
        /// This is used to copy the DTO to the biz data in. You can override this if the copy requires 
        /// extra data bindings added, e.g. from supporting dropdown lists.
        /// Note: Look at AutoMapperSetup method first as that can handle a number of mapping issues
        /// </summary>
        /// <param name="db">This allows you to access the DbContext to access the database</param>
        /// <param name="source"></param>
        /// <returns></returns>
        protected internal virtual TBizIn CopyToBizData(DbContext db, TDtoIn source)
        {
            return Mapper.Map<TBizIn>(source);
        }
    }
}