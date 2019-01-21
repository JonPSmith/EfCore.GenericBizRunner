// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.
namespace GenericBizRunner
{
    /// <summary>
    /// This is an async Action that returns a Task containing a result TOut
    /// It updates the database and therefore requires EF SaveChanges to be called to persist the changes
    /// </summary>
    /// <typeparam name="TOut">Output from the business logic</typeparam>
    public interface IGenericActionOutOnlyWriteDbAsync<TOut> : IGenericActionOutOnlyAsync<TOut>
    {}
}