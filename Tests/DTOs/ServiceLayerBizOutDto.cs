// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using AutoMapper;
using GenericBizRunner;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.BizDTOs;

namespace Tests.DTOs
{
    public class ServiceLayerBizOutDto : GenericActionFromBizDto<BizDataOut, ServiceLayerBizOutDto>
    {
        public string Output { get; set; }

        public bool SetupSecondaryOutputDataCalled { get; private set; }
        public bool CopyFromBizDataCalled { get; private set; }

        protected internal override void SetupSecondaryOutputData(DbContext db)
        {
            SetupSecondaryOutputDataCalled = true;
            base.SetupSecondaryOutputData(db);
        }

        protected internal override ServiceLayerBizOutDto CopyFromBizData(DbContext db, IMapper mapper,
            BizDataOut source)
        {
            var result = base.CopyFromBizData(db, mapper, source);
            result.CopyFromBizDataCalled = true;
            return result;
        }
    }
}