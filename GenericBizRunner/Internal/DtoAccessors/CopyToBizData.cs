#region licence

// =====================================================
// GenericActions Library - Library for running business actions
// Filename: CopyToBizData.cs
// Date Created: 2015/02/02
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================

#endregion

using Microsoft.EntityFrameworkCore;

namespace GenericBizRunner.Internal.DtoAccessors
{
    internal class CopyToBizData<TBizIn, TDtoIn>
        where TBizIn : class, new()
        where TDtoIn : GenericActionToBizDto<TBizIn, TDtoIn>, new()
    {
        public TBizIn CopyToBiz(DbContext db, object source)
        {
            return ((TDtoIn) source).CopyToBizData(db, (TDtoIn) source);
        }

        public void SetupSecondaryData(DbContext db, object dto)
        {
            ((TDtoIn) dto).SetupSecondaryData(db);
        }
    }
}