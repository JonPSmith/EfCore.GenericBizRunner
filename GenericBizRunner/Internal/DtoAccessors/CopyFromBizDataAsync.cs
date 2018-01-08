// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.DtoAccessors
{
    internal class CopyFromBizDataAsync<TBizOut, TDtoOut>
        where TBizOut : class
        where TDtoOut : GenericActionFromBizDtoAsync<TBizOut, TDtoOut>, new()
    {
        private readonly TDtoOut _dtoInstance = new TDtoOut();

        public async Task<TDtoOut> CopyFromBizAsync(DbContext db, IMapper mapper, object source)
        {
            return await _dtoInstance.CopyFromBizDataAsync(db, mapper, (TBizOut) source).ConfigureAwait(false);
        }

        public async Task SetupSecondaryOutputDataAsync(DbContext db, object dto)
        {
            await ((TDtoOut) dto).SetupSecondaryOutputDataAsync(db).ConfigureAwait(false);
        }
    }
}