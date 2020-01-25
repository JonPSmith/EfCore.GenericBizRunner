// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using AutoMapper;
using GenericBizRunner;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using TestBizLayer.BizDTOs;

namespace Tests.DTOs
{
    public class ServiceLayerBizInDtoAsync : GenericActionToBizDtoAsync<BizDataIn, ServiceLayerBizInDtoAsync>
    {
        public int Num { get; set; }

        public bool RaiseErrorInSetupSecondaryData { get; set; }

        public bool SetupSecondaryDataCalled { get; private set; }
        public bool CopyToBizDataCalled { get; private set; }

        protected internal override async Task SetupSecondaryDataAsync(DbContext db, IBizActionStatus status)
        {
            SetupSecondaryDataCalled = true;
            if (RaiseErrorInSetupSecondaryData)
                status.AddError("Error in SetupSecondaryData");
        }

        protected internal override async Task<BizDataIn> CopyToBizDataAsync(DbContext db, IMapper mapper, ServiceLayerBizInDtoAsync source)
        {
            CopyToBizDataCalled = true;
            return await base.CopyToBizDataAsync(db, mapper, source);
        }
    }
}