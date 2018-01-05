#region licence

// =====================================================
// GenericActions Library - Library for running business actions
// Filename: IGenericActionInOnlyWriteDbAsync.cs
// Date Created: 2015/02/03
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================

#endregion

namespace GenericBizRunner
{
    /// <summary>
    /// This is an Action that takes an input and returns a Task
    /// It updates the database and therefore requires EF SaveChanges to be called to persist the changes
    /// </summary>
    /// <typeparam name="TIn">Input to the business logic</typeparam>
    public interface IGenericActionInOnlyWriteDbAsync<in TIn> : IGenericActionInOnlyAsync<TIn>
    {
    }
}