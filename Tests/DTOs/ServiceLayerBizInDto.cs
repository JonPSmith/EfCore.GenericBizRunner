// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using AutoMapper;
using GenericBizRunner;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.BizDTOs;

namespace Tests.DTOs
{
    public class ServiceLayerBizInDto : GenericActionToBizDto<BizDataIn, ServiceLayerBizInDto>
    {
        public int Num { get; set; }

        public bool RaiseErrorInSetupSecondaryData { get; set; }

        public bool SetupSecondaryDataCalled { get; private set; }

        public bool CopyToBizDataCalled { get; private set; }

        protected internal override void SetupSecondaryData(DbContext db, IBizActionStatus status)
        {
            SetupSecondaryDataCalled = true;
            if (RaiseErrorInSetupSecondaryData)
                status.AddError("Error in SetupSecondaryData");
        }

        protected internal override BizDataIn CopyToBizData(DbContext db, IMapper mapper, ServiceLayerBizInDto source)
        {
            CopyToBizDataCalled = true;
            return base.CopyToBizData(db, mapper, source);
        }
    }
}