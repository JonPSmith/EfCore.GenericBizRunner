#region licence

// =====================================================
// GenericActions Library - Library for running business actions
// Filename: IGenericActionOutOnlyWriteDbAsync.cs
// Date Created: 2015/02/03
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================

#endregion

namespace GenericBizRunner
{
    /// <summary>
    /// This is an async Action that returns a Task containing a result TOut
    /// It updates the database and therefore requires EF SaveChanges to be called to persist the changes
    /// </summary>
    /// <typeparam name="TOut">Output from the business logic</typeparam>
    public interface IGenericActionOutOnlyWriteDbAsync<TOut> : IGenericActionOutOnlyAsync<TOut>
    {
    }
}