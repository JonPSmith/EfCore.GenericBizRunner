// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner
{
    /// <summary>
    /// Placing this on a class will tell GenericActions that you want the method called SetupSecondaryData to be called.
    /// This interface is not needed when the class inherits from GenericAction...Dto as that already forces that
    /// </summary>
    public interface ISetupSecondaryDataAsync
    {
        /// <summary>
        /// This method will be run when GetDto, ResetDto or an error occurs in a in an action
        /// </summary>
        /// <param name="db"></param>
        Task SetupSecondaryDataAsync(DbContext db);
    }
}