// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using AutoMapper;
using GenericBizRunner;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.BizDTOs;

namespace Tests.DTOs
{
    public class ServiceLayerBizInDto : GenericActionToBizDto<BizDataIn, ServiceLayerBizInDto>
    {
        public int Num { get; set; }

        public bool SetupSecondaryDataCalled { get; private set; }

        public bool CopyToBizDataCalled { get; private set; }

        protected internal override void SetupSecondaryData(DbContext db)
        {
            SetupSecondaryDataCalled = true;
        }

        protected internal override BizDataIn CopyToBizData(DbContext db, IMapper mapper, ServiceLayerBizInDto source)
        {
            CopyToBizDataCalled = true;
            return base.CopyToBizData(db, mapper, source);
        }
    }
}