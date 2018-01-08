// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.DtoAccessors
{
    internal class CopyToBizData<TBizIn, TDtoIn>
        where TBizIn : class, new()
        where TDtoIn : GenericActionToBizDto<TBizIn, TDtoIn>, new()
    {
        public TBizIn CopyToBiz(DbContext db, IMapper mapper, object source)
        {
            return ((TDtoIn) source).CopyToBizData(db, mapper, (TDtoIn) source);
        }

        public void SetupSecondaryData(DbContext db, object dto)
        {
            ((TDtoIn) dto).SetupSecondaryData(db);
        }
    }
}