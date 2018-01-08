// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.
namespace GenericBizRunner
{
    /// <summary>
    /// This is an Action that takes an input and returns void
    /// It updates the database and therefore requires EF SaveChanges to be called to persist the changes
    /// </summary>
    /// <typeparam name="TIn">Input to the business logic</typeparam>
    public interface IGenericActionInOnlyWriteDb<in TIn> : IGenericActionInOnly<TIn>
    {}
}