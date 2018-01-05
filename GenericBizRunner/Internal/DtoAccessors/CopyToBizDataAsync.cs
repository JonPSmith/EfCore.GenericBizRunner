using System.Threading.Tasks;
using GenericActions;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.DtoAccessors
{
    internal class CopyToBizDataAsync<TBizIn, TDtoIn>
        where TBizIn : class, new()
        where TDtoIn : GenericActionToBizDtoAsync<TBizIn, TDtoIn>, new()
    {
        public async Task<TBizIn> CopyToBizAsync(DbContext db, object source)
        {
            return await ((TDtoIn) source).CopyToBizDataAsync(db, (TDtoIn) source).ConfigureAwait(false);
        }

        public async Task SetupSecondaryDataAsync(DbContext db, object dto)
        {
            await ((TDtoIn) dto).SetupSecondaryDataAsync(db).ConfigureAwait(false);
        }
    }
}