// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using AutoMapper;
using GenericActions;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.DtoAccessors
{
    internal class CopyToBizDataAsync<TBizIn, TDtoIn>
        where TBizIn : class, new()
        where TDtoIn : GenericActionToBizDtoAsync<TBizIn, TDtoIn>, new()
    {
        public async Task<TBizIn> CopyToBizAsync(DbContext db, IMapper mapper, object source)
        {
            return await ((TDtoIn)source).CopyToBizDataAsync(db, mapper, (TDtoIn)source).ConfigureAwait(false);
        }

        public async Task SetupSecondaryDataAsync(DbContext db, object dto)
        {
            await ((TDtoIn) dto).SetupSecondaryDataAsync(db).ConfigureAwait(false);
        }
    }
}