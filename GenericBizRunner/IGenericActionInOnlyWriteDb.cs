#region licence
// =====================================================
// GenericActions Library - Library for running business actions
// Filename: IGenericActionInOnlyWriteDb.cs
// Date Created: 2015/02/03
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

namespace GenericBizRunner
{
    /// <summary>
    /// This is an Action that takes an input and returns void
    /// It updates the database and therefore requires EF SaveChanges to be called to persist the changes
    /// </summary>
    /// <typeparam name="TIn">Input to the business logic</typeparam>
    public interface IGenericActionInOnlyWriteDb<in TIn> : IGenericActionInOnly<TIn>
    {

    }
}