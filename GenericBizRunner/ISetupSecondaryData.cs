using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner
{
    /// <summary>
    /// Placing this on a class will tell GenericActions that you want the method called SetupSecondaryData to be called.
    /// This interface is not needed when the class inherits from GenericAction...Dto as that already forces that
    /// </summary>
    public interface ISetupSecondaryData
    {
        /// <summary>
        /// This method will be run when GetDto, ResetDto or an error occurs in a in an action
        /// </summary>
        /// <param name="db"></param>
        void SetupSecondaryData(DbContext db);
    }
}