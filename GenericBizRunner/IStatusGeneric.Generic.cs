
namespace GenericBizRunner
{
    /// <summary>
    /// This allows a a result to be returned with a status
    /// Not used by GenericBizRunner but you might like to use it when building your business logic
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStatusGeneric<T> : IStatusGeneric
    {
        /// <summary>
        /// This contains the return result, or if there are errors it will retunr default(T)
        /// </summary>
        T Result { get; set; }
    }
}