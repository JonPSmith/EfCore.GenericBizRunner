
namespace GenericBizRunner
{
    public interface IStatusGeneric<T> : IStatusGeneric
    {
        /// <summary>
        /// This contains the return result, or if there are errors it will retunr default(T)
        /// </summary>
        T Result { get; set; }
    }
}