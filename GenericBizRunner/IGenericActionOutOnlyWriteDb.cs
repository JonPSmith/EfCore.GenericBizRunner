namespace GenericBizRunner
{
    /// <summary>
    /// This is an Action that returns a result TOut
    /// It updates the database and therefore requires EF SaveChanges to be called to persist the changes
    /// </summary>
    /// <typeparam name="TOut">Output from the business logic</typeparam>
    public interface IGenericActionOutOnlyWriteDb<TOut> : IGenericActionOutOnly<TOut>
    {
    }
}